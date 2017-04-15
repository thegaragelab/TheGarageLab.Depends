using Xunit;

namespace TheGarageLab.Depends.Test
{
    /// <summary>
    /// Test parent/child behaviour
    /// </summary>
    public class TestChildren
    {
        /// <summary>
        /// A registration with a child overrides the parent
        /// registration.
        /// </summary>
        [Fact]
        public void ChildRegistrationOverridesParent()
        {
            using (var resolver = new Resolver())
            {
                resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
                using (var child = resolver.CreateChild())
                {
                    child.Register(typeof(TestCases.IService2), typeof(TestCases.AlternativeImplementationOfIService2));
                    var value = child.Resolve(typeof(TestCases.IService2));
                    Assert.Equal(typeof(TestCases.AlternativeImplementationOfIService2), value.GetType());
                }
            }
        }

        /// <summary>
        /// If the child registration has a different lifetime that
        /// is used instead of the parent setting.
        /// </summary>
        [Fact]
        public void ChildLifetimeOverridesParent()
        {
            using (var resolver = new Resolver())
            {
                resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2), Lifetime.Transient);
                using (var child = resolver.CreateChild())
                {
                    child.Register(typeof(TestCases.IService2), typeof(TestCases.AlternativeImplementationOfIService2), Lifetime.Singleton);
                    var value1 = child.Resolve(typeof(TestCases.IService2));
                    var value2 = child.Resolve(typeof(TestCases.IService2));
                    Assert.Equal(value1, value2);
                }
            }
        }

        /// <summary>
        /// If a registration is not present in the child the
        /// parent registration will be used.
        /// </summary>
        [Fact]
        public void ChildDefersToParent()
        {
            using (var resolver = new Resolver())
            {
                resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
                using (var child = resolver.CreateChild())
                {
                    var value = child.Resolve(typeof(TestCases.IService2));
                    Assert.Equal(typeof(TestCases.ImplementationOfIService2), value.GetType());
                }
            }
        }

        /// <summary>
        /// If neither the parent or child have a registration
        /// the resolution will fail.
        /// </summary>
        [Fact]
        public void ResolutionFailsWithNoRegistrations()
        {
            using (var resolver = new Resolver())
            {
                using (var child = resolver.CreateChild())
                {
                    Assert.Throws<NoImplementationSpecifiedForInterfaceException>(() => { child.Resolve(typeof(TestCases.IService2)); });
                }
            }
        }
    }
}
