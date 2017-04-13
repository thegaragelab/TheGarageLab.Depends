using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGarageLab.Depends
{
    public static class Extensions
    {
        /// <summary>
        /// Generic version of type registration
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="resolver"></param>
        /// <param name="lifetime"></param>
        public static void Register<T1, T2>(this IResolver resolver, Lifetime lifetime = Lifetime.Transient)
        {
            resolver.Register(typeof(T1), typeof(T2), lifetime);
        }

        /// <summary>
        /// Generic version of type registration.
        /// </summary>
        /// <param name="resolver">Resolver.</param>
        /// <param name="singleton">Singleton.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Register<T>(this IResolver resolver, T singleton)
        {
            resolver.Register(typeof(T), singleton);
        }

        /// <summary>
        /// Generic version of type registration
        /// </summary>
        /// <param name="resolver">Resolver.</param>
        /// <param name="factory">Factory.</param>
        /// <param name="lifetime">Lifetime.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Register<T>(this IResolver resolver, Func<IResolver, T> factory, Lifetime lifetime = Lifetime.Transient)
        {
            resolver.Register(typeof(T), (r) => { return (T)factory(r); }, lifetime);
        }

        /// <summary>
        /// Generic version of type resolution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static T Resolve<T>(this IResolver resolver)
        {
            return (T)resolver.Resolve(typeof(T));
        }
    }
}
