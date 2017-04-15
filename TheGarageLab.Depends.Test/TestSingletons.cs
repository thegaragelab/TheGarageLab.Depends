using Xunit;

namespace TheGarageLab.Depends.Test
{
    public class TestSingletons
    {
        /// <summary>
        /// When an implementation is registered as a singleton the
        /// same instance is returned for each call to resolve.
        /// </summary>
        [Fact]
        public void SameInstanceReturnedForSingletons()
        {
            var resolver = new Resolver();
            resolver.Register(typeof(TestCases.IService1), typeof(TestCases.ImplementationOfIService1), Lifetime.Singleton);
            var i1 = resolver.Resolve(typeof(TestCases.IService1));
            var i2 = resolver.Resolve(typeof(TestCases.IService1));
            Assert.Equal(i1, i2);
        }
    }
}
