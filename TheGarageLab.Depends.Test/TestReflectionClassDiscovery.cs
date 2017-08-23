using System;
using System.Reflection;
using System.Linq;
using Xunit;

namespace TheGarageLab.Depends.Test
{
    /// <summary>
    /// Attribute class used for testing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class ClassTestAttribute1 : Attribute { }

    /// <summary>
    /// Attribute class used for testing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class ClassTestAttribute2 : Attribute { }

    /// <summary>
    /// Attribute class used for testing
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class ClassTestAttribute3 : Attribute { }

    // Test classes for discovery
    [ClassTestAttribute1]
    internal class ClassWithAttribute1 { }

    [ClassTestAttribute2]
    internal class ClassWithAttribute2 { }

    [ClassTestAttribute1]
    [ClassTestAttribute2]
    internal class ClassWithAttribute1AndAttribute2 { }

    /// <summary>
    /// Test the reflection helper methods.
    /// </summary>
    public class TestReflectionClassDiscovery
    {
        /// <summary>
        /// Get the current assembly (used for testing). In this case use the assembly
        /// that contains this test class.
        /// </summary>
        /// <returns></returns>
        private Assembly GetCurrentAssembly()
        {
            return GetType().Assembly;
        }

        /// <summary>
        /// Make sure we can find all classes with TestAttribute1
        /// </summary>
        [Fact]
        public void WillFindAllClassesWithAttribute1()
        {
            var results = ReflectionHelpers.FindClassesWithAttribute<ClassTestAttribute1>(GetCurrentAssembly());
            Assert.NotNull(results);
            Assert.Equal(2, results.Keys.Count);
            Assert.True(results.Keys.Contains(typeof(ClassWithAttribute1)));
            Assert.True(results.Keys.Contains(typeof(ClassWithAttribute1AndAttribute2)));
        }

        /// <summary>
        /// Make sure we can find all classes with TestAttribute2
        /// </summary>
        [Fact]
        public void WillFindAllClassesWithAttribute2()
        {
            var results = ReflectionHelpers.FindClassesWithAttribute<ClassTestAttribute2>(GetCurrentAssembly());
            Assert.NotNull(results);
            Assert.Equal(2, results.Keys.Count);
            Assert.True(results.Keys.Contains(typeof(ClassWithAttribute2)));
            Assert.True(results.Keys.Contains(typeof(ClassWithAttribute1AndAttribute2)));
        }

        /// <summary>
        /// If no classes are marked with the requested attribute we get a non-null but
        /// empty result.
        /// </summary>
        [Fact]
        public void WillReturnEmptyResultIfNoMarkedClasses()
        {
            var results = ReflectionHelpers.FindClassesWithAttribute<ClassTestAttribute3>(GetCurrentAssembly());
            Assert.NotNull(results);
            Assert.Equal(0, results.Keys.Count);
        }
    }
}
