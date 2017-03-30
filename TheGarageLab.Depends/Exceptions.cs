using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Exception thrown when a class registered for an interface does not
    /// implement that interface.
    /// </summary>
    public class ClassDoesNotImplementInterfaceException : Exception
    {
    }

    /// <summary>
    /// Exception throw when there is no class registered as an implementation
    /// for the given interface.
    /// </summary>
    public class NoImplementationSpecifiedForInterfaceException : Exception
    {
    }

    /// <summary>
    /// Exception thrown when a suitable constructor could not be found to
    /// perform the injection.
    /// </summary>
    public class NoSuitableConstructorFoundException : Exception
    {
    }

    /// <summary>
    /// Exception thrown when multiple default implementations of a specific
    /// interface have been found.
    /// </summary>
    public class MultipleDefaultImplementationsException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MultipleDefaultImplementationsException() : base() { }

        /// <summary>
        /// Constructor with the interface being provided with a default
        /// </summary>
        /// <param name="iface"></param>
        public MultipleDefaultImplementationsException(Type iface) : base(iface.ToString()) { }
    }

}
