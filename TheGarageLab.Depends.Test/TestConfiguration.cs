using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TheGarageLab.Depends.Test
{
    public class TestConfiguration
    {
        /// <summary>
        /// A configuration containing an empty or invalid
        /// class name will fail.
        /// </summary>
        [Fact]
        public void InvalidTargetNameFails()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>()
            {
                new InjectionConfiguration()
                {
                    Target = "",
                    Implementation = typeof(TestCases.ImplementationOfIService2).AssemblyQualifiedName
                }
            };
            using (var resolver = new Resolver())
            {
                Assert.Throws<TypeLoadException>(() => { resolver.Configure(config); });
            }
        }

        /// <summary>
        /// A configuration containing a null target name
        /// will fail.
        /// </summary>
        [Fact]
        public void NullTargetNameFails()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>()
            {
                new InjectionConfiguration()
                {
                    Target = null,
                    Implementation = typeof(TestCases.ImplementationOfIService2).AssemblyQualifiedName
                }
            };
            using (var resolver = new Resolver())
            {
                Assert.Throws<ArgumentNullException>(() => { resolver.Configure(config); });
            }
        }

        /// <summary>
        /// A configuration containing an empty or invalid
        /// implementation name will fail.
        /// </summary>
        [Fact]
        public void InvalidImplementationNameFails()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>()
            {
                new InjectionConfiguration()
                {
                    Target = typeof(TestCases.IService2).AssemblyQualifiedName,
                    Implementation = ""
                }
            };
            using (var resolver = new Resolver())
            {
                Assert.Throws<TypeLoadException>(() => { resolver.Configure(config); });
            }
        }

        /// <summary>
        /// A configuration containing a null implementation
        /// name will fail.
        /// </summary>
        [Fact]
        public void NullImplementationNameFails()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>()
            {
                new InjectionConfiguration()
                {
                    Target = typeof(TestCases.IService2).AssemblyQualifiedName,
                    Implementation = null
                }
            };
            using (var resolver = new Resolver())
            {
                Assert.Throws<ArgumentNullException>(() => { resolver.Configure(config); });
            }
        }

        /// <summary>
        /// A configuration containing a transient mapping
        /// will succeed
        /// </summary>
        [Fact]
        public void TransientConfigurationsSucceed()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>()
            {
                new InjectionConfiguration()
                {
                    Target = typeof(TestCases.IService2).AssemblyQualifiedName,
                    Implementation = typeof(TestCases.ImplementationOfIService2).AssemblyQualifiedName,
                    Lifetime = Lifetime.Transient
                }
            };
            using (var resolver = new Resolver())
            {
                resolver.Configure(config);
                var result = resolver.Resolve(typeof(TestCases.IService2));
                Assert.Equal(typeof(TestCases.ImplementationOfIService2), result.GetType());
            }
        }

        /// <summary>
        /// A configuration containing a singleton mapping
        /// will succeed.
        /// </summary>
        [Fact]
        public void SingletonConfigurationsSucceed()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>()
            {
                new InjectionConfiguration()
                {
                    Target = typeof(TestCases.IService2).AssemblyQualifiedName,
                    Implementation = typeof(TestCases.ImplementationOfIService2).AssemblyQualifiedName,
                    Lifetime = Lifetime.Singleton
                }
            };
            using (var resolver = new Resolver())
            {
                resolver.Configure(config);
                var r1 = resolver.Resolve(typeof(TestCases.IService2));
                var r2 = resolver.Resolve(typeof(TestCases.IService2));
                Assert.Equal(r1, r2);
            }
        }

        /// <summary>
        /// A configuration containing an empty list will
        /// succeed.
        /// </summary>
        [Fact]
        public void EmptyListSucceeds()
        {
            List<InjectionConfiguration> config = new List<InjectionConfiguration>();
            using (var resolver = new Resolver())
            {
                resolver.Configure(config);
            }
        }

        /// <summary>
        /// A null configuration will fail.
        /// </summary>
        [Fact]
        public void NullListFails()
        {
            List<InjectionConfiguration> config = null;
            using (var resolver = new Resolver())
            {
                Assert.Throws<ArgumentException>(() => { resolver.Configure(config); });
            }
        }
    }
}
