using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TheGarageLab.Depends.Test
{
    public class TestResolve
    {
        /// <summary>
        /// If a class type is passed to resolver it should create an instance of that class
        /// </summary>
        [Fact]
        public void WillInstantiateFromClassType()
        {
            var resolver = new DependencyResolver();
            var result = resolver.Resolve(typeof(TestCases.ImplementationOfIService1));
            Assert.NotNull(result);
            Assert.Equal(typeof(TestCases.ImplementationOfIService1), result.GetType());
        }

        /// <summary>
        /// If a class type is passed to resolver that has dependencies those dependencies should
        /// be resolved.
        /// </summary>
        [Fact]
        public void WillInstantiateFromClassTypeAndInject()
        {
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// If a class is marked as DefaultImplementation for an interface and no other
        /// registrations are made that is the class that will be returned.
        /// </summary>
        [Fact]
        public void WillInstantiateDefaultImplementation()
        {
            var resolver = new DependencyResolver();
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
            var resolver = new DependencyResolver();
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
            var resolver = new DependencyResolver();
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
            var resolver = new DependencyResolver();
            // Verify there is not a default implementation
            Assert.Throws<NoImplementationSpecifiedForInterfaceException>(() => { resolver.Resolve(typeof(TestCases.IService2)); });
        }

    }
}
