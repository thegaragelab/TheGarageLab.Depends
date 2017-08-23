using System;
using Xunit;
using Moq;

namespace TheGarageLab.Depends.Test
{
    /// <summary>
    /// Test the extension methods
    /// </summary>
    public class TestResolverExtensions
    {
        /// <summary>
        /// Default class registration lifetime is Transient
        /// </summary>
        [Fact]
        public void RegisterClassDefaultTransient()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Register<TestCases.IService2, TestCases.ImplementationOfIService2>();
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    typeof(TestCases.ImplementationOfIService2),
                    Lifetime.Transient),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure class registration passes lifetime.
        /// </summary>
        [Fact]
        public void RegisterClassTransient()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Register<TestCases.IService2, TestCases.ImplementationOfIService2>(Lifetime.Transient);
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    typeof(TestCases.ImplementationOfIService2),
                    Lifetime.Transient),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure class registration passes lifetime.
        /// </summary>
        [Fact]
        public void RegisterClassSingleton()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Register<TestCases.IService2, TestCases.ImplementationOfIService2>(Lifetime.Singleton);
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    typeof(TestCases.ImplementationOfIService2),
                    Lifetime.Singleton),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure singleton registration invokes correct method
        /// </summary>
        [Fact]
        public void RegisterSingletonInstance()
        {
            var mock = new Mock<IResolver>();
            var singleton = new TestCases.ImplementationOfIService2();
            mock.Object.Register<TestCases.IService2>(singleton);
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    singleton),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure default factory registration is Transient
        /// </summary>
        [Fact]
        public void RegisterFactoryDefaultTransient()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Register<TestCases.IService2>((r) => { return (TestCases.ImplementationOfIService2)r.Resolve(typeof(TestCases.ImplementationOfIService2)); });
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    It.IsAny<Func<IResolver, object>>(),
                    Lifetime.Transient),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure factory registration passes lifetime
        /// </summary>
        [Fact]
        public void RegisterFactoryTransient()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Register<TestCases.IService2>((r) => { return (TestCases.ImplementationOfIService2)r.Resolve(typeof(TestCases.ImplementationOfIService2)); }, Lifetime.Transient);
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    It.IsAny<Func<IResolver, object>>(),
                    Lifetime.Transient),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure factory registration passes lifetime
        /// </summary>
        [Fact]
        public void RegisterFactorySingleton()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Register<TestCases.IService2>((r) => { return (TestCases.ImplementationOfIService2)r.Resolve(typeof(TestCases.ImplementationOfIService2)); }, Lifetime.Singleton);
            mock.Verify(resolver =>
                resolver.Register(
                    typeof(TestCases.IService2),
                    It.IsAny<Func<IResolver, object>>(),
                    Lifetime.Singleton),
                Times.Once()
            );
        }

        /// <summary>
        /// Ensure generic resolve invokes correct method
        /// </summary>
        [Fact]
        public void ResolveByType()
        {
            var mock = new Mock<IResolver>();
            mock.Object.Resolve<TestCases.IService2>();
            mock.Verify(resolver => resolver.Resolve(typeof(TestCases.IService2)), Times.Once());
        }
    }
}
