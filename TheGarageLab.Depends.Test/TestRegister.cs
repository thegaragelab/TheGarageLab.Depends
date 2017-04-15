using System;
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
            var resolver = new Resolver();
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
            var resolver = new Resolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(typeof(TestCases.ImplementationOfIService3), typeof(TestCases.AlternativeImplementationOfIService3)); });
        }

        /// <summary>
        /// Resolver will not register an interface as an implementation
        /// even if it is assignable to the target
        /// </summary>
        [Fact]
        public void WillNotRegisterChildInterfaceAsImplementation()
        {
            var resolver = new Resolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(typeof(TestCases.IService3), typeof(TestCases.IService3Extended)); });
        }

        /// <summary>
        /// Resolver will not register an implementation that does not
        /// implement the target interface
        /// </summary>
        [Fact]
        public void WillNotRegisterNonImplementingClass()
        {
            var resolver = new Resolver();
            Assert.Throws<ClassDoesNotImplementInterfaceException>(() => { resolver.Register(typeof(TestCases.IService3), typeof(TestCases.ImplementationOfIService2)); });
        }

        /// <summary>
        /// Resolver will not register if the target is null
        /// </summary>
        [Fact]
        public void WillNotRegisterForNullInterface()
        {
            var resolver = new Resolver();
            Assert.Throws<ArgumentNullException>(() => { resolver.Register(null, typeof(TestCases.ImplementationOfIService3)); });
        }

        /// <summary>
        /// Resolver will not register if the class is null
        /// </summary>
        [Fact]
        public void WillNotRegisterForNullClass()
        {
            var resolver = new Resolver();
            Assert.Throws<ArgumentNullException>(() => { resolver.Register(typeof(TestCases.IService3), (Type)null); });
        }

        /// <summary>
        /// Resolver will register an object instance as a singleton
        /// </summary>
        [Fact]
        public void WillRegisterSingleton()
        {
            var resolver = new Resolver();
            var singleton = new TestCases.ImplementationOfIService2();
            resolver.Register(typeof(TestCases.IService2), singleton);
            Assert.Equal(singleton, resolver.Resolve(typeof(TestCases.IService2)));
        }

        /// <summary>
        /// Resolver will not register an object instance if it does not
        /// implement the target class.
        /// </summary>
        [Fact]
        public void WillNotRegisterSingletonOfWrongClass()
        {
            var resolver = new Resolver();
            var singleton = new TestCases.ImplementationOfIService1();
            Assert.Throws<ClassDoesNotImplementInterfaceException>(() => { resolver.Register(typeof(TestCases.IService2), singleton); });
        }

        /// <summary>
        /// Registering a singleton with a null type will fail.
        /// </summary>
        [Fact]
        public void WillNotRegisterSingletonWithNullType()
        {
            var resolver = new Resolver();
            var singleton = new TestCases.ImplementationOfIService1();
            Assert.Throws<ArgumentException>(() => { resolver.Register(null, singleton); });
        }

        /// <summary>
        /// Registering a null singleton fails
        /// </summary>
        [Fact]
        public void WillNotRegisterNullSingleton()
        {
            var resolver = new Resolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(typeof(TestCases.IService2), (object)null); });
        }

        /// <summary>
        /// Will register a factory function to create instances
        /// </summary>
        [Fact]
        public void WillRegisterFactoryFunction()
        {
            var resolver = new Resolver();
            resolver.Register(typeof(TestCases.IService2), (r) => { return r.Resolve(typeof(TestCases.ImplementationOfIService2)); });
        }

        /// <summary>
        /// Registering a factory with a null type will fail
        /// </summary>
        [Fact]
        public void WillNotRegisterFactoryWithNullType()
        {
            var resolver = new Resolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(null, (r) => { return r.Resolve(typeof(TestCases.ImplementationOfIService2)); }); });
        }

        /// <summary>
        /// Registering a null factory will fail
        /// </summary>
        [Fact]
        public void WillNotRegisterNullFactory()
        {
            var resolver = new Resolver();
            Assert.Throws<ArgumentException>(() => { resolver.Register(typeof(TestCases.IService2), (Func<IResolver, object>)null); });
        }
    }
}