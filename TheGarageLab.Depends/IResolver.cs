using System;
using System.Collections.Generic;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Defines the public interface to a dependency resolver.
    /// </summary>
    public interface IResolver : IDisposable
    {
        /// <summary>
        /// Register an implementing class for the interface (or parent class)
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="cls"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        void Register(Type iface, Type cls, Lifetime lifetime = Lifetime.Transient);

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
        void Register(Type iface, Func<IResolver, object> factory, Lifetime lifetime = Lifetime.Transient);

        /// <summary>
        /// Resolve an interface to it's required concrete implementation.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        object Resolve(Type t);

        /// <summary>
        /// Configure this instance from an external configuration.
        /// </summary>
        /// <param name="config"></param>
        void Configure(IReadOnlyList<InjectionConfiguration> config);

        /// <summary>
        /// Create a child resolver. The child will maintain it's
        /// own configuration but delegate to the parent for any
        /// unregistered types.
        /// </summary>
        /// <returns></returns>
        IResolver CreateChild();
    }
}