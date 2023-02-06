using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DependencyInjection
{
    public static partial class Extensions
    {
        /// <summary>
        /// 自动依赖注入
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AutoInject(this IServiceCollection services)
        {
            services.TryAddSingleton<IAssemblyLoader>(_ => new AllAssemblyLoader());
            services.TryAddSingleton<IServiceDescriptorProvider>(_ => new DefaultServiceDescriptorProvider(services.GetInstanceOrNull<IAssemblyLoader>())); ;
            var provider = services.GetInstanceOrNull<IServiceDescriptorProvider>();
            var serviceDescriptors = provider.GetServiceDescriptors();

            foreach (var serviceDescriptor in serviceDescriptors)
                services.Add(serviceDescriptor);

            //循环依赖问题处理
            services.AddTransient(typeof(Lazy<>), typeof(LazilyResolved<>));
            return services;
        }


        /// <summary>
        /// 获取单例注册服务对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="services">服务集合</param>
        private static T GetInstanceOrNull<T>(this IServiceCollection services, ServiceLifetime lifeTime = ServiceLifetime.Singleton)
        {
            var descriptor =
                services.FirstOrDefault(x => x.ServiceType == typeof(T) && x.Lifetime == lifeTime);
            if (descriptor?.ImplementationInstance != null)
                return (T)descriptor.ImplementationInstance;
            if (descriptor?.ImplementationFactory != null)
                return (T)descriptor.ImplementationFactory.Invoke(null);
            return default;
        }
    }
}
