using System;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends.Factories
{
    internal class FunctionFactory : AbstractFactory
    {
        /// <summary>
        /// The class or interface we are creating instances of
        /// </summary>
        private readonly Type ForClass;

        /// <summary>
        /// The factory function itself
        /// </summary>
        private readonly Func<IResolver, object> FactoryFunc;

        /// <summary>
        /// Constructor with required values
        /// </summary>
        /// <param name="forType">For type.</param>
        /// <param name="factoryFunc">Factory.</param>
        /// <param name="lifetime">Lifetime.</param>
        public FunctionFactory (Type forType, Func<IResolver, object> factoryFunc, Lifetime lifetime) : base(lifetime)
        {
            ForClass = forType;
            FactoryFunc = factoryFunc;
        }

        /// <summary>
        /// Create a new instance of the class injecting all dependencies
        /// as required.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        protected override object Factory(IResolver resolver)
        {
            // Create (and verify) the new instance
            object result = Factory(resolver);
            Ensure.IsNotNull<ObjectConstructionFailedException>(result);
            Ensure.IsTrue<ClassDoesNotImplementInterfaceException>(ForClass.IsAssignableFrom(result.GetType()));
            // All done
            return result;
        }

    }
}

