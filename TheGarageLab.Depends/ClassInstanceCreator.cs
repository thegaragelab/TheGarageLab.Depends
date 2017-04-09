using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    internal class ClassInstanceCreator : IInstanceCreator
    {
        private readonly Type ForClass;
        private readonly Lifetime Lifetime;
        private object Singleton;

        public ClassInstanceCreator(Type forClass, Lifetime lifetime)
        {
            ForClass = forClass;
            Lifetime = lifetime;
        }

        #region Helpers
        /// <summary>
        /// Find a constructor that has only Interface parameters
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
        #endregion

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
            // Find the constructor and list the arguments
            ConstructorInfo ctor = FindAppropriateConstructor(ForClass);
            Ensure.IsNotNull<UnableToDetermineInjectionPointException>(ctor);
            // Recursivley create the required dependency arguments
            var parameters = ctor.GetParameters();
            object[] args = new object[parameters.Length];
            for (int p = 0; p < parameters.Length; p++)
                args[p] = resolver.GetResolverFor(ForClass).Resolve(parameters[p].ParameterType);
            // Create the object (and save it if it is a singleton)
            object result = Activator.CreateInstance(ForClass, args);
            if (Lifetime == Lifetime.Singleton)
                Singleton = result;
            // All done
            return result;
        }
    }
}
