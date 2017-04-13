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
    internal class SingletonFactory : AbstractFactory
    {
        /// <summary>
        /// Constructor with the singleton to return for this
        /// creator
        /// </summary>
        /// <param name="singleton"></param>
        public SingletonFactory(object singleton) : base(Lifetime.Singleton)
        {
            Singleton = singleton;
        }

        /// <summary>
        /// Return the pre-created instance.
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        protected override object Factory(IResolver resolver)
        {
            // This method should never be called
            throw new NotImplementedException();
        }
    }
}
