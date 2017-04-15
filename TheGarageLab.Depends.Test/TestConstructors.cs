using Xunit;

namespace TheGarageLab.Depends.Test
{
    /// <summary>
    /// Unit tests for injection constructors
    /// </summary>
    public class TestConstructors
    {
        /// <summary>
        /// If the class has an explicit default constructor only it will
        /// be used for creation.
        /// </summary>
        [Fact]
        public void WillUseDefaultConstructor()
        {
            var resolver = new Resolver();
            var instance = resolver.Resolve(typeof(TestCases.ContainerWithExplicitDefaultConstructor));
            Assert.Equal(typeof(TestCases.ContainerWithExplicitDefaultConstructor), instance.GetType());
            Assert.True((instance as TestCases.ContainerWithExplicitDefaultConstructor).DefaultConstructorInvoked);
        }

        /// <summary>
        /// If the class has no explicit constructors the automatically
        /// generated default constructor will be used.
        /// </summary>
        [Fact]
        public void WillUseAutomaticDefaultConstructor()
        {
            var resolver = new Resolver();
            var instance = resolver.Resolve(typeof(TestCases.ContainerWithAutomaticDefaultConstructor));
            Assert.Equal(typeof(TestCases.ContainerWithAutomaticDefaultConstructor), instance.GetType());
        }

        /// <summary>
        /// Will use a constructor that has a mix of interfaces and classes
        /// in the parameter list.
        /// </summary>
        [Fact]
        public void WillUseConstructorWithClassParameters()
        {
            var resolver = new Resolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            var instance = resolver.Resolve(typeof(TestCases.ContainerWithClassInConstructor));
            Assert.NotNull(instance);
            Assert.NotNull((instance as TestCases.ContainerWithClassInConstructor).Container);
            Assert.Equal(
                typeof(TestCases.ContainerWithAutomaticDefaultConstructor),
                (instance as TestCases.ContainerWithClassInConstructor).Container.GetType()
                );
            Assert.NotNull((instance as TestCases.ContainerWithClassInConstructor).Service1);
            Assert.Equal(
                typeof(TestCases.ImplementationOfIService1),
                (instance as TestCases.ContainerWithClassInConstructor).Service1.GetType()
                );
        }

        /// <summary>
        /// Will fail if there is more than one eligble constructor and
        /// none of them are marked as the injection point.
        /// </summary>
        [Fact]
        public void WillFailIfMoreThanOneConstructor()
        {
            var resolver = new Resolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
            Assert.Throws<MultipleInjectionPointsException>(() => { resolver.Resolve(typeof(TestCases.ContainerWithMultipleConstructors)); });
        }

        /// <summary>
        /// If there is more than one eligible constructor the one
        /// marked with the [Injector] attribute will be used.
        /// </summary>
        [Fact]
        public void WillUseInjectorAttribute()
        {
            var resolver = new Resolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
            var instance = resolver.Resolve(typeof(TestCases.ContainerWithMultipleConstructorsAndAttribute));
            Assert.NotNull(instance);
            Assert.Equal(typeof(TestCases.ContainerWithMultipleConstructorsAndAttribute), instance.GetType());
            Assert.NotNull((instance as TestCases.ContainerWithMultipleConstructorsAndAttribute).Service1);
            Assert.Null((instance as TestCases.ContainerWithMultipleConstructorsAndAttribute).Service2);
        }

        /// <summary>
        /// Will fail if there is more than one constructor marked with
        /// the [Injector] attribute.
        /// </summary>
        [Fact]
        public void WilFailIfMoreThanOneMarkedConstructor()
        {
            var resolver = new Resolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
            Assert.Throws<MultipleInjectionPointsException>(
                () =>
                {
                    resolver.Resolve(typeof(TestCases.ContainerWithMultipleConstructorsAndMultipleAttributes));
                });
        }
    }
}
