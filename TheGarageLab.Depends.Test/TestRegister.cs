using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TheGarageLab.Depends.Test
{

    public class TestRegister
    {
        /// <summary>
        /// Resolver will map a class to an interface for resolution
        /// </summary>
        [Fact]
        public void WillRegisterForAnInterface()
        {
            var resolver = new DependencyResolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            var created = resolver.Resolve(typeof(TestCases.IService1));
            Assert.Equal(typeof(TestCases.ImplementationOfIService1), created.GetType());
        }

        /// <summary>
        /// Resolver will not register an implementation for a class target.
        /// </summary>
        [Fact]
        public void WillNotRegisterForAClass()
        {
            var resolver = new DependencyResolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(typeof(TestCases.ImplementationOfIService3), typeof(TestCases.AlternativeImplementationOfIService3)); });
        }

        /// <summary>
        /// Resolver will not register an interface as an implementation
        /// even if it is assignable to the target
        /// </summary>
        [Fact]
        public void WillNotRegisterChildInterfaceAsImplementation()
        {
            var resolver = new DependencyResolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(typeof(TestCases.IService3), typeof(TestCases.IService3Extended)); });
        }

        /// <summary>
        /// Resolver will not register an implementation that does not
        /// implement the target interface
        /// </summary>
        [Fact]
        public void WillNotRegisterNonImplementingClass()
        {
            var resolver = new DependencyResolver();
            Assert.Throws<ClassDoesNotImplementInterfaceException>(() => { resolver.Register(typeof(TestCases.IService3), typeof(TestCases.ImplementationOfIService2)); });
        }

        /// <summary>
        /// Resolver will not register if the target is null
        /// </summary>
        [Fact]
        public void WillNotRegisterForNullInterface()
        {
            var resolver = new DependencyResolver();
            Assert.Throws<ArgumentNullException>(() => { resolver.Register(null, typeof(TestCases.ImplementationOfIService3)); });
        }

        /// <summary>
        /// Resolver will not register if the class is null
        /// </summary>
        [Fact]
        public void WillNotRegisterForNullClass()
        {
            var resolver = new DependencyResolver();
            Assert.Throws<ArgumentNullException>(() => { resolver.Register(typeof(TestCases.IService3), (Type)null); });
        }
    }
}