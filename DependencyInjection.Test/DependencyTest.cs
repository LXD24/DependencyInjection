using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DependencyInjection.Test
{
    public class DependencyTest
    {

        private readonly IServiceProvider _serviceProvider;

        public DependencyTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AutoInject();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }



        [Fact]
        public void SelfDependency_Test()
        {
            var instance = _serviceProvider.GetService<Test>();
            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonDependency_Test()
        {
            var scope1 = _serviceProvider.CreateScope();
            var instance1 = scope1.ServiceProvider.GetRequiredService<ITestSingleton>();

            var scope2 = _serviceProvider.CreateScope();
            var instance2 = scope2.ServiceProvider.GetRequiredService<ITestSingleton>();

            Assert.Equal(instance1.GetHashCode(), instance2.GetHashCode());
        }



        [Fact]
        public void ScopeDependency_Test()
        {
            var scope1 = _serviceProvider.CreateScope();
            var instance1 = scope1.ServiceProvider.GetRequiredService<ITestScope>();
            var instance2 = scope1.ServiceProvider.GetRequiredService<ITestScope>();

            var scope2 = _serviceProvider.CreateScope();
            var instance3 = scope2.ServiceProvider.GetRequiredService<ITestScope>();

            Assert.Equal(instance1.GetHashCode(), instance2.GetHashCode());
            Assert.NotEqual(instance2.GetHashCode(), instance3.GetHashCode());
        }


        [Fact]
        public void TransientDependency_Test()
        {
            var scope1 = _serviceProvider.CreateScope();
            var instance1 = scope1.ServiceProvider.GetRequiredService<ITestTransient>();
            var instance2 = scope1.ServiceProvider.GetRequiredService<ITestTransient>();
            Assert.NotEqual(instance1.GetHashCode(), instance2.GetHashCode());
        }

        [Fact]
        public void AbstractDependency_Test()
        {
            var instance1 = _serviceProvider.GetRequiredService<ITestAbstract>();
            Assert.Equal(nameof(TestAbstractExtends), instance1.GetType().Name);
        }


        [Fact]
        public void MutiImplementDependency_Test()
        {
            var instances = _serviceProvider.GetRequiredService<IEnumerable<ITestMutiImplement>>();
            Assert.Equal(2, instances.Count());
        }

        [Fact]
        public void MutiInterfaceDependency_Test()
        {
            var scope1 = _serviceProvider.CreateScope();
            var instanceA = scope1.ServiceProvider.GetRequiredService<ITestA>();
            var instanceB = scope1.ServiceProvider.GetRequiredService<ITestB>();
            Assert.Equal(instanceA.GetHashCode(), instanceB.GetHashCode());
        }

        [Fact]
        public void ReplaceService_Test()
        {
            var instance = _serviceProvider.GetRequiredService<ITestC>();
            Assert.Equal(typeof(TestReplaceServices), instance.GetType());


            var instances = _serviceProvider.GetRequiredService<IEnumerable<ITestC>>();
            Assert.Single(instances);

        }


        [Fact]
        public void TryRegister_Test()
        {
            var instance = _serviceProvider.GetRequiredService<ITestD>();
            Assert.Equal(typeof(TestD), instance.GetType());

            var instances = _serviceProvider.GetRequiredService<IEnumerable<ITestD>>();
            Assert.Single(instances);
        }
    }
}
