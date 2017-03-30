﻿using System;
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
    public class UnableToDetermineInjectionPointException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UnableToDetermineInjectionPointException() : base() { }

        /// <summary>
        /// Constructor with the interface being provided with a default
        /// </summary>
        /// <param name="iface"></param>
        public UnableToDetermineInjectionPointException(Type iface) : base(iface.FullName) { }
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
        public MultipleDefaultImplementationsException(Type iface) : base(iface.FullName) { }
    }

    /// <summary>
    /// Exception thrown when multiple injection points where found in a class
    /// </summary>
    public class MultipleInjectionPointsException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MultipleInjectionPointsException() { }

        /// <summary>
        /// Constructor with the class the error was found in
        /// </summary>
        /// <param name="forClass"></param>
        public MultipleInjectionPointsException(Type forClass) : base(forClass.FullName) { }
    }
}
