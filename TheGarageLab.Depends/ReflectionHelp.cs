using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Helper methods to perform reflection related tasks
    /// </summary>
    public static class ReflectionHelp
    {
        /// <summary>
        /// Find all types in the list that are classes and have the given attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Dictionary<Type, List<CustomAttributeData>> FindClassesWithAttribute<T>(IEnumerable<Type> types) where T : Attribute
        {
            var results = new Dictionary<Type, List<CustomAttributeData>>();
            foreach (var candidate in types.Where(t => t.IsClass() && t.CustomAttributes().Where(c => c.AttributeType == typeof(T)).Any()))
                results[candidate] = candidate.CustomAttributes().Where(c => c.AttributeType == typeof(T)).ToList();
            return results;
        }

        /// <summary>
        /// Find all methods on a class that are decorated with the specified attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<MethodInfo, List<CustomAttributeData>> FindMethodsWithAttribute<T>(Type t) where T : Attribute
        {
            Ensure.IsNotNull(t);
            Ensure.IsTrue(t.IsClass());
            var results = new Dictionary<MethodInfo, List<CustomAttributeData>>();
            // TODO: Implement this
            return results;
        }
    }
}
