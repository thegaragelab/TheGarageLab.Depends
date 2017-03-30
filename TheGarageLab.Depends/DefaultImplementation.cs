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
}