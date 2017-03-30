using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGarageLab.Depends.Test
{
    /// <summary>
    /// Various test classes to use in the tests, they are shared so it makes
    /// sense to put them in a single spot.
    /// </summary>
    public class TestCases
    {
        public interface IService1 { }

        public interface IService2 { }

        public interface IService3 { }

        public class ImplementationOfIService1 : IService1 { }

        public class AlternativeImplementationOfIService1 : IService1 { }

        [DefaultImplementation(typeof(IService1))]
        public class DefaultImplementationOfIService1 : IService1 { }

        public class ImplementationOfIService2 : IService2 { }

        public class AlternativeImplementationOfIService2 : IService2 { }

        public class ImplementationOfIService3 : IService3 { }

        public class AlternativeImplementationOfIService3 : IService3 { }
    }
}
