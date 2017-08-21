using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Helper methods to perform reflection related tasks
    /// </summary>
    public static class ReflectionHelp
    {
        /// <summary>
        /// Get all assemblies that depend on the named assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName)
        {
            Ensure.IsNotNullOrWhiteSpace(assemblyName);
            List<Assembly> results = new List<Assembly>();
            foreach (var library in DependencyContext.Default.RuntimeLibraries.Where(lib => lib.Dependencies.Where(dep => dep.Name.StartsWith(assemblyName)).Any()))
                results.Add(Assembly.Load(new AssemblyName(library.Name)));
            return results;

        }

        /// <summary>
        /// Get all assemblies that depend on the referenced assembly
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetReferencingAssemblies(Assembly target)
        {
            Ensure.IsNotNull(target);
            return GetReferencingAssemblies(target.GetName().Name);
        }
    }
}
