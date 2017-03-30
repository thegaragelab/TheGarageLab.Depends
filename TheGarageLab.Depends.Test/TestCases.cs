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

        public class ContainerWithDependencies
        {
            /// <summary>
            /// Expose the created service
            /// </summary>
            public IService1 Service { get; private set; }

            /// <summary>
            /// Constructor with injections
            /// </summary>
            /// <param name="service"></param>
            public ContainerWithDependencies(IService1 service)
            {
                Service = service;
            }
        }

        public class ContainerWithMultipleConstructors
        {
            // Expose Service1 instance
            public IService1 Service1 { get; private set; }

            // Expose Service2 instance
            public IService2 Service2 { get; private set; }

            /// <summary>
            /// Constructor with multiple services
            /// </summary>
            /// <param name="service1"></param>
            /// <param name="service2"></param>
            public ContainerWithMultipleConstructors(IService1 service1, IService2 service2)
            {
                Service1 = service1;
                Service2 = service2;
            }

            /// <summary>
            /// Constructor with injections
            /// </summary>
            /// <param name="service"></param>
            public ContainerWithMultipleConstructors(IService1 service1) : this(service1, null) { }

        }
    }
}
