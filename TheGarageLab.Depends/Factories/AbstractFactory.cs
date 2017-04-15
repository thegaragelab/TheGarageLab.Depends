﻿using System;

namespace TheGarageLab.Depends.Factories
{
    /// <summary>
    /// Base class for instance factories. This provides a single point
    /// for all common behaviour which simplifies the implementation of
    /// the various factories.
    /// </summary>
    internal abstract class AbstractFactory : IDisposable
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        protected object Singleton { get; set; }

        /// <summary>
        /// The objects lifetime
        /// </summary>
        private readonly Lifetime Lifetime;

        /// <summary>
        /// Constructor with a lifetime.
        /// </summary>
        /// <param name="lifetime"></param>
        protected AbstractFactory(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// Create a new instance or return the singleton
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public object CreateInstance(IResolver resolver)
        {
            // Just return the singleton if we have one
            if (Singleton != null)
                return Singleton;
            // Use the factory function to create the instance
            object result = Factory(resolver);
            if (Lifetime == Lifetime.Singleton)
                Singleton = result;
            // All done
            return result;
        }

        /// <summary>
        /// The actual factory method. Provided by child classes.
        /// </summary>
        /// <returns></returns>
        protected abstract object Factory(IResolver resolver);

        /// <summary>
        /// Dispose of any disposable singletons we have created.
        /// </summary>
        public void Dispose()
        {
            var disposable = Singleton as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
