using System;
using System.Linq;
using System.Reflection;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    internal class ClassFactory : AbstractFactory
    {
        private readonly Type ForClass;

        /// <summary>
        /// Constructor with class type and lifetime
        /// </summary>
        /// <param name="forClass"></param>
        /// <param name="lifetime"></param>
        public ClassFactory(Type forClass, Lifetime lifetime) : base(lifetime)
        {
            ForClass = forClass;
        }

        /// <summary>
        /// Find a suitable constructor (public, optionally marked with 'Injector')
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private ConstructorInfo FindAppropriateConstructor(Type t)
        {
            var candidates = t.GetConstructors().Where(b => b.IsPublic);
            if (candidates.Count() == 1)
                return candidates.First();
            // Look for one that has the 'Injector' attributes
            var injectable = candidates.Where(b => b.CustomAttributes.Where(c => c.AttributeType == typeof(Injector)).Count() > 0);
            if (injectable.Count() == 1)
                return injectable.First();
            // Could not determine injection point
            return null;
        }

        /// <summary>
        /// Create a new instance of the class injecting all dependencies
        /// as required.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        protected override object Factory(IResolver resolver)
        {
            // Find the constructor and list the arguments
            ConstructorInfo ctor = FindAppropriateConstructor(ForClass);
            Ensure.IsNotNull<UnableToDetermineInjectionPointException>(ctor);
            // Recursivley create the required dependency arguments
            var parameters = ctor.GetParameters();
            object[] args = new object[parameters.Length];
            for (int p = 0; p < parameters.Length; p++)
                args[p] = resolver.Resolve(parameters[p].ParameterType);
            // Create the object
            return Activator.CreateInstance(ForClass, args);
        }
    }
}
