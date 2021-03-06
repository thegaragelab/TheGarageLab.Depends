﻿using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using TheGarageLab.Ensures;
using TheGarageLab.Depends.Factories;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Implementation of a dependency resolver node.
    /// </summary>
    public class Resolver : IResolver
    {
        /// <summary>
        /// Used to record information about default implementations
        /// </summary>
        private class ImplementationInformation
        {
            public Type Implementation;
            public Lifetime Lifetime;
        }

        // Lock object to control access to the default mappings
        private static object m_defaultLock = new object();

        // The actual default mappings, lazy initialised on first access
        private static Dictionary<Type, ImplementationInformation> m_defaults;

        /// <summary>
        /// Provide access to the shared mapping of default implementations
        /// (classes marked with the DefaultImplementation attribute)
        /// </summary>
        private static Dictionary<Type, ImplementationInformation> Defaults
        {
            get
            {
                // This is global state so make sure multiple
                // threads don't try and modify it at the same time.
                Monitor.Enter(m_defaultLock);
                if (m_defaults == null)
                    m_defaults = FindDefaultRegistrations();
                Monitor.Exit(m_defaultLock);
                return m_defaults;
            }
        }

        /// <summary>
        /// Reference to the parent resolver
        /// </summary>
        protected Resolver Parent { get; private set; }

        /// <summary>
        /// Child resolvers
        /// </summary>
        private HashSet<Resolver> Children;

        /// <summary>
        /// Map interfaces to instance creators
        /// </summary>
        private Dictionary<Type, AbstractFactory> Implementations;

        /// <summary>
        /// Set to true when the object has been disposed.
        /// </summary>
        private bool Disposed;

        #region Helpers
        /// <summary>
        /// Determine if the class is a valid registration for the interface.
        /// 
        /// Must meet the following requirements:
        ///   1. Neither type is null
        ///   2. The iface parameter is an interface
        ///   3. The cls parameter is a class
        ///   2. The interface is assignable from the class
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        private static void TestRegistrationTypes(Type iface, Type cls)
        {
            Ensure.IsNotNull<ArgumentNullException>(iface);
            Ensure.IsTrue(iface.IsInterface);
            Ensure.IsNotNull<ArgumentNullException>(cls);
            Ensure.IsTrue(cls.IsClass);
            Ensure.IsTrue<ClassDoesNotImplementInterfaceException>(iface.IsAssignableFrom(cls));
        }

        /// <summary>
        /// Examines all loaded assemblies for classes marked as 
        /// DefaultImplementation for an interface
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Type, ImplementationInformation> FindDefaultRegistrations()
        {
            Dictionary<Type, ImplementationInformation> results = new Dictionary<Type, ImplementationInformation>();
            // Scan all loaded classes for default implementations
            IEnumerator enumerator = Thread.GetDomain().GetAssemblies().GetEnumerator();
            while (enumerator.MoveNext())
            {
                try
                {
                    Type[] types = ((Assembly)enumerator.Current).GetTypes();
                    if (!((Assembly)enumerator.Current).FullName.StartsWith("System."))
                    {
                        IEnumerator enumerator2 = types.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Type current = (Type)enumerator2.Current;
                            if (current.IsAbstract || current.IsInterface)
                                continue;
                            foreach (var attribute in current.CustomAttributes)
                            {
                                if (attribute.AttributeType == typeof(DefaultImplementation))
                                {
                                    Type target = attribute.ConstructorArguments[0].Value as Type;
                                    Lifetime lifetime = (Lifetime)attribute.ConstructorArguments[1].Value;
                                    TestRegistrationTypes(target, current);
                                    if (results.ContainsKey(target))
                                        throw new MultipleDefaultImplementationsException(target);
                                    results[target] = new ImplementationInformation()
                                    {
                                        Implementation = current,
                                        Lifetime = lifetime
                                    };
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine("Failed to enumerate types from assemblies. More information ...");
                    foreach (var e in ex.LoaderExceptions)
                        Console.WriteLine("  {0}", e.ToString());
                }
            }
            return results;
        }

        /// <summary>
        /// Walk the resolver tree to find an appropriate instance
        /// creator for this class.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private AbstractFactory FindCreatorFor(Type t)
        {
            // Check for creator at this level
            AbstractFactory creator;
            if ((Implementations != null) && Implementations.TryGetValue(t, out creator))
                return creator;
            // Ask the the parent for a creator
            if (Parent != null)
                return Parent.FindCreatorFor(t);
            // Finally, see if it can be created directly
            if (!t.IsClass || t.IsAbstract)
                throw new NoImplementationSpecifiedForInterfaceException();
            return new ClassFactory(t, Lifetime.Transient);
        }

        /// <summary>
        /// Map an instance creator to a type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        private void RegisterCreator(Type t, AbstractFactory creator)
        {
            // Safely add to this resolvers mapping and create a child resolver for it
            Monitor.Enter(this);
            // Map the implementation
            if (Implementations == null)
                Implementations = new Dictionary<Type, AbstractFactory>();
            Implementations[t] = creator;
            Monitor.Exit(this);
        }
        #endregion

        #region Parent/Child Management
        /// <summary>
        /// Add a new child to this resolver.
        /// </summary>
        /// <param name="resolver"></param>
        protected void AddChild(Resolver resolver)
        {
            Monitor.Enter(this);
            if (Children == null)
                Children = new HashSet<Resolver>();
            Children.Add(resolver);
            Monitor.Exit(this);
        }

        /// <summary>
        /// Remove a child from this resolver
        /// </summary>
        /// <param name="resolver"></param>
        protected void RemoveChild(Resolver resolver)
        {
            Monitor.Enter(this);
            try
            {
                if ((Children == null) || !Children.Contains(resolver))
                    throw new InvalidOperationException("Attempting to remove unregistered child resolver.");
                Children.Remove(resolver);
            }
            catch(Exception)
            {
                throw;
            }
            finally {
                Monitor.Exit(this);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Internal constructor used for cloning
        /// </summary>
        /// <param name="parent"></param>
        protected Resolver(Resolver parent)
        {
            Parent = parent;
            Parent?.AddChild(this);
        }

        /// <summary>
        /// Public constructor for root elements
        /// </summary>
        public Resolver() : this(null)
        {
            // This is a root resolver, add the default dependencies
            foreach (Type target in Defaults.Keys)
            {
                var info = Defaults[target];
                RegisterCreator(target, new ClassFactory(info.Implementation, info.Lifetime));
            }
        }
        #endregion

        #region Implementation of IResolver
        /// <summary>
        /// Register a class as the preferred implementation of a specific interface.
        /// 
        /// The class must implement the interface and will override any existing
        /// registration.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="cls"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public void Register(Type iface, Type cls, Lifetime lifetime = Lifetime.Transient)
        {
            Ensure.IsFalse<InvalidOperationException>(Disposed);
            TestRegistrationTypes(iface, cls);
            RegisterCreator(iface, new ClassFactory(cls, lifetime));
        }

        /// <summary>
        /// Register a singleton instance for the interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="singleton"></param>
        /// <returns></returns>
        public void Register(Type iface, object singleton)
        {
            Ensure.IsFalse<InvalidOperationException>(Disposed);
            // Check parameters
            Ensure.IsNotNull(iface);
            Ensure.IsNotNull(singleton);
            Ensure.IsTrue<ClassDoesNotImplementInterfaceException>(iface.IsAssignableFrom(singleton.GetType()));
            // Register the creator
            RegisterCreator(iface, new SingletonFactory(singleton));
        }

        /// <summary>
        /// Register a factory function for the interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="factory"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public void Register(Type iface, Func<IResolver, object> factory, Lifetime lifetime = Lifetime.Transient)
        {
            Ensure.IsFalse<InvalidOperationException>(Disposed);
            // Check parameters
            Ensure.IsNotNull(iface);
            Ensure.IsNotNull (factory);
            // Register the creator
            RegisterCreator(iface, new FunctionFactory(iface, factory, lifetime));
        }

        /// <summary>
        /// Create and return an instance of the given interface resolving all
        /// dependencies as specified in the implementations constructor.
        /// </summary>
        /// <param name="iface"></param>
        /// <returns></returns>
        public object Resolve(Type iface)
        {
            Ensure.IsFalse<InvalidOperationException>(Disposed);
            Ensure.IsNotNull<ArgumentNullException>(iface);
            return FindCreatorFor(iface).CreateInstance(this);
        }

        /// <summary>
        /// Configure this instance from an external configuration.
        /// </summary>
        /// <param name="config"></param>
        public void Configure(IReadOnlyList<InjectionConfiguration> config)
        {
            // Check parameters
            Ensure.IsNotNull(config);
            // Add each entry
            foreach (var injection in config)
            {
                Type t1 = Type.GetType(injection.Target, true, false);
                Type t2 = Type.GetType(injection.Implementation, true, false);
                Register(t1, t2, injection.Lifetime);
            }
        }

        /// <summary>
        /// Create a child resolver. The child will maintain it's
        /// own configuration but delegate to the parent for any
        /// unregistered types.
        /// </summary>
        /// <returns></returns>
        public IResolver CreateChild()
        {
            Ensure.IsFalse<InvalidOperationException>(Disposed);
            return new Resolver(this);
        }

        /// <summary>
        /// Dispose of the object
        /// </summary>
        public void Dispose()
        {
            Ensure.IsFalse<InvalidOperationException>(Disposed);
            Disposed = true;
            // Remove ourselves from our parent (if we have one)
            if (Parent != null)
                Parent.RemoveChild(this);
            // Dispose all of the child elements
            if (Children != null)
            {
                // Use a copy of the collection, it will be modified
                // during the operation.
                foreach (var child in new List<IResolver>(Children))
                    child.Dispose();
            }
            // Now dispose of all our singleton instances
            if (Implementations != null)
                foreach (var disposable in Implementations.Values)
                    disposable.Dispose();
        }
        #endregion
    }
}