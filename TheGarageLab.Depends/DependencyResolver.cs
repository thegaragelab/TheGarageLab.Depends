using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Implementation of a dependency resolver node.
    /// </summary>
    public class DependencyResolver : IResolver
    {
        // Lock object to control access to the default mappings
        private static object m_defaultLock = new object();

        // The actual default mappings, lazy initialised on first access
        private static Dictionary<Type, IInstanceCreator> m_defaults;

        /// <summary>
        /// Provide access to the shared mapping of default implementations
        /// (classes marked with the DefaultImplementation attribute)
        /// </summary>
        private static Dictionary<Type, IInstanceCreator> Defaults
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
        /// The parent resolver. The root of a specific dependency tree will not
        /// have a parent.
        /// </summary>
        protected DependencyResolver Parent { get; private set; }

        /// <summary>
        /// Map interfaces to instance creators
        /// </summary>
        private Dictionary<Type, IInstanceCreator> Implementations;

        /// <summary>
        /// Map child resolvers to implementors
        /// </summary>
        private Dictionary<Type, DependencyResolver> Resolvers;

        /// <summary>
        /// Constructor with a parent. This is only used internally to
        /// build the dependency tree.
        /// </summary>
        /// <param name="parent"></param>
        protected DependencyResolver(DependencyResolver parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Default constructor
        /// 
        /// This is used to create the root node of a dependency tree.
        /// </summary>
        public DependencyResolver() : this(null) { }

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
        private static Dictionary<Type, IInstanceCreator> FindDefaultRegistrations()
        {
            Dictionary<Type, IInstanceCreator> results = new Dictionary<Type, IInstanceCreator>();
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
                                    results[target] = new ClassInstanceCreator(current, lifetime);
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
        private IInstanceCreator FindCreatorFor(Type t)
        {
            // Check for creator at this level
            IInstanceCreator creator;
            if ((Implementations != null) && Implementations.TryGetValue(t, out creator))
                return creator;
            // Check the parent for a creator
            if (Parent != null)
                return Parent.FindCreatorFor(t);
            // Check defaults
            if (Defaults.TryGetValue(t, out creator))
                return creator;
            // Finally, see if it can be created directly
            if (!t.IsClass || t.IsAbstract)
                throw new NoImplementationSpecifiedForInterfaceException();
            return new ClassInstanceCreator(t, Lifetime.Transient);
        }

        /// <summary>
        /// Map an instance creator to a type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        private void RegisterCreator(Type t, IInstanceCreator creator)
        {
            // Safely add to this resolvers mapping and create a child resolver for it
            Monitor.Enter(this);
            // Map the implementation
            if (Implementations == null)
                Implementations = new Dictionary<Type, IInstanceCreator>();
            Implementations[t] = creator;
            Monitor.Exit(this);
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
        public IResolver Register(Type iface, Type cls, Lifetime lifetime = Lifetime.Transient)
        {
            TestRegistrationTypes(iface, cls);
            RegisterCreator(iface, new ClassInstanceCreator(cls, lifetime));
            return GetResolverFor(cls);
        }

        /// <summary>
        /// Register a singleton instance for the interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="singleton"></param>
        /// <returns></returns>
        public void Register(Type iface, object singleton)
        {
            // Check parameters
            Ensure.IsNotNull(iface);
            Ensure.IsNotNull(singleton);
            Ensure.IsTrue<ClassDoesNotImplementInterfaceException>(iface.IsAssignableFrom(singleton.GetType()));
            // Register the creator
            RegisterCreator(iface, new SingletonInstanceCreator(singleton));
        }

        /// <summary>
        /// Register a factory function for the interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="factory"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public void Register(Type iface, Func<object> factory, Lifetime lifetime = Lifetime.Transient)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the resolver for a specific class
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public IResolver GetResolverFor(Type cls)
        {
            // Check arguments
            Ensure.IsNotNull<ArgumentNullException>(cls);
            Ensure.IsTrue(cls.IsClass);
            // Safely fetch or create a resolver
            Monitor.Enter(this);
            if (Resolvers == null)
                Resolvers = new Dictionary<Type, DependencyResolver>();
            if (!Resolvers.ContainsKey(cls))
                Resolvers[cls] = new DependencyResolver(this);
            Monitor.Exit(this);
            return Resolvers[cls];
        }

        /// <summary>
        /// Create and return an instance of the given interface resolving all
        /// dependencies as specified in the implementations constructor.
        /// </summary>
        /// <param name="iface"></param>
        /// <returns></returns>
        public object Resolve(Type iface)
        {
            Ensure.IsNotNull<ArgumentNullException>(iface);
            return FindCreatorFor(iface).CreateInstance(this);
        }
        #endregion
    }
}