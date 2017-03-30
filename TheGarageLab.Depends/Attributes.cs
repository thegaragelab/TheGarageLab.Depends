using System;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// A class attribute used to identify default implementations of an
    /// interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultImplementation : Attribute
    {
        /// <summary>
        /// Indicate the type this implementation is the default for
        /// </summary>
        /// <param name="forType"></param>
        public DefaultImplementation(Type forType) { }
    }

    /// <summary>
    /// This attribute is used to determine the appropriate constructor
    /// to use for injection if there are multiple candidates available.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class Injector : Attribute
    {
        public Injector() { }
    }
}