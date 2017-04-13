using System;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    internal class FactoryInstanceCreator : IInstanceCreator
    {
        private readonly Type ForClass;
        private readonly Lifetime Lifetime;
        private readonly Func<IResolver, object> Factory;
        private object Singleton;

        /// <summary>
        /// Constructor with required values
        /// </summary>
        /// <param name="forType">For type.</param>
        /// <param name="factory">Factory.</param>
        /// <param name="lifetime">Lifetime.</param>
        public FactoryInstanceCreator (Type forType, Func<IResolver, object> factory, Lifetime lifetime)
        {
            ForClass = forType;
            Factory = factory;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Create a new instance of the class injecting all dependencies
        /// as required.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public object CreateInstance(IResolver resolver)
        {
            // If we have the singleton, just return it
            if ((Lifetime == Lifetime.Singleton) && (Singleton != null))
                return Singleton;
            // Create (and verify) the new instance
            object result = Factory(resolver);
            Ensure.IsNotNull<ObjectConstructionFailedException>(result);
            Ensure.IsTrue<ClassDoesNotImplementInterfaceException>(ForClass.IsAssignableFrom(result.GetType()));
            // If we are creating a singleton, stash it away
            if (Lifetime == Lifetime.Singleton)
                Singleton = result;
            // All done
            return result;
        }

    }
}

