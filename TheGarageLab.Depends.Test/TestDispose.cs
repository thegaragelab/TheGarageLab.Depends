using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TheGarageLab.Depends.Test
{
    /// <summary>
    /// Test the IDisposable implementation
    /// </summary>
    public class TestDispose
    {
        /// <summary>
        /// Ensure that calling dispose makes future operations
        /// invalid.
        /// </summary>
        [Fact]
        public void DisposeInvalidatesObject()
        {
            IResolver resolver;
            using (resolver = new Resolver())
            {
                resolver.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
            }
            Assert.Throws<InvalidOperationException>(() => { resolver.Resolve(typeof(TestCases.IService2)); });
        }

        /// <summary>
        /// Disposing a resolver with a child ensures that child has been
        /// disposed.
        /// </summary>
        [Fact]
        public void DoesDisposeChildren()
        {
            IResolver parent, child;
            using (parent = new Resolver())
            {
                child = parent.CreateChild();
            }
            Assert.Throws<InvalidOperationException>(() => { child.Resolve(typeof(TestCases.ImplementationOfIService2)); });
        }

        /// <summary>
        /// If a disposable singleton was created that instance
        /// will be disposed with the resolver.
        /// </summary>
        [Fact]
        public void DoesDisposeSingletons()
        {
            var singleton = new TestCases.ImplementationOfIDisposableService();
            using (var resolver = new Resolver())
            {
                resolver.Register(typeof(TestCases.IDisposableService), singleton);
            }
            Assert.True(singleton.Disposed);
        }

        /// <summary>
        /// If a disposable transient was created it will
        /// not be disposed when the resolver is disposed.
        /// </summary>
        [Fact]
        public void DoesNotDisposeTransients()
        {
            TestCases.IDisposableService transient;
            using (var resolver = new Resolver())
            {
                resolver.Register(typeof(TestCases.IDisposableService), typeof(TestCases.ImplementationOfIDisposableService));
                transient = (TestCases.IDisposableService)resolver.Resolve(typeof(TestCases.IDisposableService));
            }
            Assert.False(transient.Disposed);
        }

        /// <summary>
        /// Disposing a child resolver does not dispose the
        /// parent.
        /// </summary>
        [Fact]
        public void DisposingChildDoesNotDisposeParent()
        {
            using (var resolver = new Resolver())
            {
                using (var child = resolver.CreateChild())
                {
                    child.Register(typeof(TestCases.IService2), typeof(TestCases.ImplementationOfIService2));
                }
                resolver.Register(typeof(TestCases.IService2), typeof(TestCases.AlternativeImplementationOfIService2));
            }
        }
    }
}
