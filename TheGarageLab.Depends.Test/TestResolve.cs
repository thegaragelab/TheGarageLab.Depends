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
            Assert.True(false, "Not implemented");
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
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// If there is a default implmentation and an alternative is registered
        /// manually that is the class that will be instantiated.
        /// </summary>
        [Fact]
        public void WillInstantiateRegisteredImplementation()
        {
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// If a class is registered for an interface and no default implementation
        /// is registered that class will be created.
        /// </summary>
        [Fact]
        public void WillInstantiateWithoutDefaultImplementation()
        {
            Assert.True(false, "Not implemented");
        }

        /// <summary>
        /// If there is no class registered for an interface (default or otherwise)
        /// instantiation will fail.
        /// </summary>
        [Fact]
        public void WillNotInstantiateUnregisteredInterface()
        {
            Assert.True(false, "Not implemented");
        }

    }
}
