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
    internal class ClassDoesNotImplementInterfaceException : Exception
    {
    }

    /// <summary>
    /// Exception throw when there is no class registered as an implementation
    /// for the given interface.
    /// </summary>
    internal class NoImplementationSpecifiedForInterfaceException : Exception
    {
    }

    /// <summary>
    /// Exception thrown when a suitable constructor could not be found to
    /// perform the injection.
    /// </summary>
    internal class NoSuitableConstructorFoundException : Exception
    {
    }

}
