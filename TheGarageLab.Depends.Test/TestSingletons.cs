using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            resolver.Register<TestCases.IService1, TestCases.ImplementationOfIService1>(Lifetime.Singleton);
            var i1 = resolver.Resolve<TestCases.IService1>();
            var i2 = resolver.Resolve<TestCases.IService1>();
            Assert.Equal(i1, i2);
        }
    }
}
