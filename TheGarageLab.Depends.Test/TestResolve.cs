using System.Reflection;
using System.Collections.Generic;
using Xunit;
using System;

namespace TheGarageLab.Depends.Test
{
    public class TestResolve
    {
        /// <summary>
        /// Get a list of loaded assemblies
        /// </summary>
        /// <returns></returns>
        private static List<Assembly> GetLoadedAssemblies()
        {
            return new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// If a class type is passed to resolver it should create an instance of that class
        /// </summary>
        [Fact]
        public void WillInstantiateFromClassType()
        {
            var resolver = new Resolver();
            var result = resolver.Resolve(typeof(TestCases.ImplementationOfIService1));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.ImplementationOfIService1), result.GetType());
        }

        /// <summary>
        /// If a class type is passed to resolver that has dependencies those dependencies should
        /// be resolved regardless of if they are defaults or manually registered.
        /// </summary>
        [Fact]
        public void WillInstantiateFromClassTypeAndInject()
        {
            var resolver = new Resolver(GetLoadedAssemblies());
            var result = resolver.Resolve(typeof(TestCases.ContainerWithDependencies));
            Assert.NotNull(result);
            Assert.NotNull((result as TestCases.ContainerWithDependencies).Service);
            Assert.Equal(typeof(TestCases.DefaultImplementationOfIService1), (result as TestCases.ContainerWithDependencies).Service.GetType());
        }

        /// <summary>
        /// If a class is marked as DefaultImplementation for an interface and no other
        /// registrations are made that is the class that will be returned.
        /// </summary>
        [Fact]
        public void WillInstantiateDefaultImplementation()
        {
            var resolver = new Resolver(GetLoadedAssemblies());
            var result = resolver.Resolve(typeof(TestCases.IService1));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.DefaultImplementationOfIService1), result.GetType());
        }

        /// <summary>
        /// If there is a default implmentation and an alternative is registered
        /// manually that is the class that will be instantiated.
        /// </summary>
        [Fact]
        public void WillInstantiateRegisteredImplementation()
        {
            var resolver = new Resolver(GetLoadedAssemblies());
            // Verify there is a default implementation
            var result = resolver.Resolve(typeof(TestCases.IService1));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.DefaultImplementationOfIService1), result.GetType());
            // Register a non-default implementation and resolve
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            result = resolver.Resolve(typeof(TestCases.IService1));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.ImplementationOfIService1), result.GetType());
        }

        /// <summary>
        /// If a class is registered for an interface and no default implementation
        /// is registered that class will be created.
        /// </summary>
        [Fact]
        public void WillInstantiateWithoutDefaultImplementation()
        {
            var resolver = new Resolver();
            // Verify there is not a default implementation
            Assert.Throws<NoImplementationSpecifiedForInterfaceException>(() => { resolver.Resolve(typeof(TestCases.IService2)); });
            // Register the implementation and ensure it is resolved
            resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
            var result = resolver.Resolve(typeof(TestCases.IService2));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.ImplementationOfIService2), result.GetType());
        }

        /// <summary>
        /// If there is no class registered for an interface (default or otherwise)
        /// instantiation will fail.
        /// </summary>
        [Fact]
        public void WillNotInstantiateUnregisteredInterface()
        {
            var resolver = new Resolver();
            // Verify there is not a default implementation
            Assert.Throws<NoImplementationSpecifiedForInterfaceException>(() => { resolver.Resolve(typeof(TestCases.IService2)); });
        }

        /// <summary>
        /// A singleton will be resolved to the given instance
        /// </summary>
        [Fact]
        public void WillResolveSingleton()
        {
            var resolver = new Resolver();
            var singleton = new TestCases.ImplementationOfIService2();
            resolver.Register(typeof(TestCases.IService2), singleton);
            Assert.Equal(singleton, resolver.Resolve(typeof(TestCases.IService2)));
        }

        /// <summary>
        /// Resolve will invoke the registered factory function
        /// </summary>
        [Fact]
        public void WillResolveWithFactoryFunction()
        {
            var resolver = new Resolver();
            resolver.Register(
                typeof(TestCases.IService2),
                (r) =>
                {
                    return r.Resolve(typeof(TestCases.AlternativeImplementationOfIService2));
                }
                );
            var result = resolver.Resolve(typeof(TestCases.IService2));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.AlternativeImplementationOfIService2), result.GetType());
        }

        /// <summary>
        /// If the factory function returns a null value the resolver will fail.
        /// </summary>
        [Fact]
        public void WillNotResolveWithNullFactoryResult()
        {
            var resolver = new Resolver();
            resolver.Register(
                typeof(TestCases.IService2),
                (r) =>
                {
                    return null;
                }
                );
            Assert.Throws<ObjectConstructionFailedException>(() => { resolver.Resolve(typeof(TestCases.IService2)); });
        }

        /// <summary>
        /// If the factory function returns an object of the wrong type
        /// the resolver will fail.
        /// </summary>
        [Fact]
        public void WillNotResolveWithWrongFactoryResultType()
        {
            var resolver = new Resolver();
            resolver.Register(
                typeof(TestCases.IService2),
                (r) =>
                {
                    return r.Resolve(typeof(TestCases.AlternativeImplementationOfIService1));
                }
                );
            Assert.Throws<ClassDoesNotImplementInterfaceException>(() => { resolver.Resolve(typeof(TestCases.IService2)); });
        }
    }
}
