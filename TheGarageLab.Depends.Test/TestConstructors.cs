using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// If the class has no explicit constructors the automatically
        /// generated default constructor will be used.
        /// </summary>
        [Fact]
        public void WillUseAutomaticDefaultConstructor()
        {
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// Will use a constructor that has a mix of interfaces and classes
        /// in the parameter list.
        /// </summary>
        [Fact]
        public void WillUseConstructorWithClassParameters()
        {
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// Will fail if there is more than one eligble constructor and
        /// none of them are marked as the injection point.
        /// </summary>
        [Fact]
        public void WillFailIfMoreThanOneConstructor()
        {
            var resolver = new DependencyResolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1));
            resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
            Assert.Throws<UnableToDetermineInjectionPointException>(() => { resolver.Resolve(typeof(TestCases.ContainerWithMultipleConstructors)); });
        }

        /// <summary>
        /// If there is more than one eligible constructor the one
        /// marked with the [Injector] attribute will be used.
        /// </summary>
        [Fact]
        public void WillUseInjectorAttribute()
        {
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// Will fail if there is more than one constructor marked with
        /// the [Injector] attribute.
        /// </summary>
        [Fact]
        public void WilFailIfMoreThanOneMarkedConstructor()
        {
            Assert.True(false, "Not implemented");
        }
    }
}
