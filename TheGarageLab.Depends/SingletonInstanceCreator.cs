using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGarageLab.Depends
{
    /// <summary>
    /// Instance creator for pre-created singletons.
    /// </summary>
    internal class SingletonInstanceCreator : IInstanceCreator
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private readonly object Singleton;

        /// <summary>
        /// Constructor with the singleton to return for this
        /// creator
        /// </summary>
        /// <param name="singleton"></param>
        public SingletonInstanceCreator(object singleton)
        {
            Singleton = singleton;
        }

        /// <summary>
        /// Return the pre-created instance.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public object CreateInstance(IResolver resolver)
        {
            return Singleton;
        }
    }
}
