using System;
using System.Reflection;
using System.Collections.Generic;
using TheGarageLab.Ensures;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Implement some reflection helpers to help the migration from .NET Framework
    /// to .NET Standard.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Internal helper to safely get type information
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static TypeInfo SafelyGetTypeInformation(Type t)
        {
            Ensure.IsNotNull(t);
            return t.GetTypeInfo();
        }

        /// <summary>
        /// Find all constructors available on the type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<ConstructorInfo> GetConstructors(this Type t)
        {
            return SafelyGetTypeInformation(t).DeclaredConstructors;
        }

        /// <summary>
        /// Determine if the class is assignable from another instance
        /// </summary>
        /// <param name="t"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this Type t, Type other)
        {
            return SafelyGetTypeInformation(t).IsAssignableFrom(SafelyGetTypeInformation(other));
        }

        /// <summary>
        /// Determine if the class represents an interface
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsInterface(this Type t)
        {
            return SafelyGetTypeInformation(t).IsInterface;
        }

        /// <summary>
        /// Determine if this type is a class
        /// </summary>
        /// <param name="Type"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool IsClass(this Type t)
        {
            return SafelyGetTypeInformation(t).IsClass;
        }

        /// <summary>
        /// Determine if this type is abstract or concrete
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsAbstract(this Type t)
        {
            return SafelyGetTypeInformation(t).IsAbstract;
        }

        /// <summary>
        /// Return a collection that represents the custom attributes
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IEnumerable<CustomAttributeData> CustomAttributes(this Type t)
        {
            return SafelyGetTypeInformation(t).CustomAttributes;
        }
    }
}
