using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGarageLab.Depends
{
    internal interface IInstanceCreator
    {
        /// <summary>
        /// Create the instance
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        object CreateInstance(IResolver resolver);
    }
}
