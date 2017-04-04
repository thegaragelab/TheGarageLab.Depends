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
        public static void Register<T1, T2>(this IResolver resolver)
        {
            resolver.Register(typeof(T1), typeof(T2));
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
