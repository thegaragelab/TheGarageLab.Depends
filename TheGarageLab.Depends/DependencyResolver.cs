using System;
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
        private static Dictionary<Type, Type> m_defaults;

        /// <summary>
        /// Provide access to the shared mapping of default implementations
        /// (classes marked with the DefaultImplementation attribute)
        /// </summary>
        protected static Dictionary<Type, Type> Defaults
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
        /// Map interfaces to implementing classes
        /// </summary>
        protected Dictionary<Type, Type> Implementations;

        /// <summary>
        /// Map child resolvers to implementing classes
        /// </summary>
        protected Dictionary<Type, DependencyResolver> Resolvers;

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
        private static Dictionary<Type, Type> FindDefaultRegistrations()
        {
            Dictionary<Type, Type> results = new Dictionary<Type, Type>();
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
                                    TestRegistrationTypes(target, current);
                                    if (results.ContainsKey(target))
                                        throw new MultipleDefaultImplementationsException(target);
                                    results[target] = current;
                                }
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine("Failed to enumerate types from assembiles. More information ...");
                    foreach (var e in ex.LoaderExceptions)
                        Console.WriteLine("  {0}", e.ToString());
                }
            }
            return results;
        }

        /// <summary>
        /// Determine if the constructor only has interface parameters
        /// </summary>
        /// <param name="ctor"></param>
        /// <returns></returns>
        private bool HasOnlyInterfaceParameters(ConstructorInfo ctor)
        {
            foreach (var param in ctor.GetParameters())
            {
                if (!param.ParameterType.IsInterface)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Find a constructor that has only Interface parameters
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private ConstructorInfo FindAppropriateConstructor(Type t)
        {
            foreach (var ctor in t.GetConstructors())
            {
                if (ctor.IsPublic && HasOnlyInterfaceParameters(ctor))
                    return ctor;
            }
            return null;
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
        /// <returns></returns>
        public IResolver Register(Type iface, Type cls)
        {
            TestRegistrationTypes(iface, cls);
            // Safely add to this resolvers mapping and create a child resolver for it
            Monitor.Enter(this);
            // Map the implementation
            if (Implementations == null)
                Implementations = new Dictionary<Type, Type>();
            Implementations[iface] = cls;
            // Create the child resolver
            if (Resolvers == null)
                Resolvers = new Dictionary<Type, DependencyResolver>();
            Resolvers[cls] = new DependencyResolver(this);
            Monitor.Exit(this);
            return Resolvers[cls];
        }

        /// <summary>
        /// Determine the class that implements the requested interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <returns></returns>
        public Type GetImplementationFor(Type iface)
        {
            // Verify arguments
            Ensure.IsNotNull<ArgumentNullException>(iface);
            Ensure.IsTrue(iface.IsInterface);
            // Find a mapping in the tree
            Monitor.Enter(this);
            Type implementor = null;
            if ((Implementations != null) && Implementations.ContainsKey(iface))
                implementor = Implementations[iface];
            else if (Parent != null)
                implementor = Parent.GetImplementationFor(iface);
            else if (Defaults.ContainsKey(iface))
                implementor = Defaults[iface];
            Monitor.Exit(this);
            return implementor;
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
            // Get the implementing class
            Type implementor = iface;
            if (iface.IsInterface)
            {
                implementor = GetImplementationFor(iface);
                Ensure.IsNotNull<NoImplementationSpecifiedForInterfaceException>(implementor);
            }
            // Find the constructor and list the arguments
            ConstructorInfo ctor = FindAppropriateConstructor(implementor);
            if (ctor == null) // Use a default constructor
                ctor = implementor.GetConstructor(new Type[] { });
            Ensure.IsNotNull<NoSuitableConstructorFoundException>(ctor);
            // Recursivley create the required dependency arguments
            var parameters = ctor.GetParameters();
            object[] args = new object[parameters.Length];
            for (int p = 0; p < parameters.Length; p++)
                args[p] = GetResolverFor(implementor).Resolve(parameters[p].ParameterType);
            // Finally, create the object
            return Activator.CreateInstance(implementor, args);
        }
        #endregion
    }
}