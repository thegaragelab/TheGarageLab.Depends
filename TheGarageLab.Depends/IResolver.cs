using System;
using System.Collections.Generic;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Defines the public interface to a dependency resolver.
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Register an implementing class for the interface (or parent class)
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="cls"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        IResolver Register(Type iface, Type cls, Lifetime lifetime = Lifetime.Transient);

        /// <summary>
        /// Register a singleton instance for the interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="singleton"></param>
        /// <returns></returns>
        void Register(Type iface, object singleton);

        /// <summary>
        /// Register a factory function for the interface.
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="factory"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        void Register(Type iface, Func<object> factory, Lifetime lifetime = Lifetime.Transient);

        /// <summary>
        /// Get the specific resolver to use for an implementing class. This
        /// should walk the tree up to the default implementations looking
        /// for an implementation.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        IResolver GetResolverFor(Type t);

        /// <summary>
        /// Resolve an interface to it's required concrete implementation.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        object Resolve(Type t);
    }
}