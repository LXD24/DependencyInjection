using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    internal class DefaultServiceDescriptorProvider : IServiceDescriptorProvider
    {
        private readonly IAssemblyLoader _assemblyLoader;


        public DefaultServiceDescriptorProvider(IAssemblyLoader assemblyLoader)
        {
            this._assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ServiceDescriptor> GetServiceDescriptors()
        {
            var registrants = LoadRegistrants();
            registrants = DependencyHandler(registrants);
            var descriptors = registrants.Select(r => r.ImplementationFactory != null
                     ? new ServiceDescriptor(r.ServiceType, r.ImplementationFactory.Invoke, r.Lifetime)
                     : new ServiceDescriptor(r.ServiceType, r.ImplementationType, r.Lifetime)).ToList();
            return descriptors;
        }

        private List<ServiceDescriptorRegistrant> LoadRegistrants()
        {
            var assemblies = _assemblyLoader.LoadAssemblies();
            var baseTypes = new[]
                { typeof(ISingletonDependency), typeof(IScopedDependency), typeof(ITransientDependency) };
            var dependencyTypes = assemblies
                .SelectMany(assembly =>
                    assembly
                        .GetTypes()
                        .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && !type.IsGenericType
                                       && (baseTypes.Any(b => b.IsAssignableFrom(type)))))
                .ToArray();


            List<ServiceDescriptorRegistrant> Registrants = new List<ServiceDescriptorRegistrant>();
            foreach (var dependencyType in dependencyTypes)
            {
                if (dependencyType.IsAbstract || dependencyType.IsInterface)
                    continue;

                ServiceLifetime? lifetime = null;
                if (typeof(ITransientDependency).IsAssignableFrom(dependencyType))
                    lifetime = ServiceLifetime.Transient;
                if (typeof(IScopedDependency).IsAssignableFrom(dependencyType))
                    lifetime = ServiceLifetime.Scoped;
                if (typeof(ISingletonDependency).IsAssignableFrom(dependencyType))
                    lifetime = ServiceLifetime.Singleton;

                if (lifetime == null)
                    continue;

                var exceptInterfaces = new[]
                {
                    typeof(IDisposable)
                };

                var interfaceTypes = dependencyType.GetInterfaces()
                    .Where(x => !exceptInterfaces.Contains(x) && !IsIgnoreInjection(x))
                    .ToArray();

                if (interfaceTypes.Length == 0)
                {
                    Registrants.Add(new ServiceDescriptorRegistrant(dependencyType, dependencyType, lifetime.Value));
                    continue;
                }

                for (var i = 0; i < interfaceTypes.Length; i++)
                {
                    var serviceType = interfaceTypes[i];
                    if (i == 0)
                    {
                        Registrants.Add(new ServiceDescriptorRegistrant(serviceType, dependencyType, lifetime.Value));
                    }
                    else
                    {
                        // 当前类实现多接口时，保证各接口注入获取到的是同一实例
                        Registrants.Add(new ServiceDescriptorRegistrant(serviceType, dependencyType, sp => sp.GetRequiredService(interfaceTypes.First()), lifetime.Value));
                    }
                }
            }
            return Registrants;
        }


        private List<ServiceDescriptorRegistrant> DependencyHandler(List<ServiceDescriptorRegistrant> registrants)
        {
            List<ServiceDescriptorRegistrant> result = new List<ServiceDescriptorRegistrant>();
            foreach (var registrant in registrants)
            {
                var serviceType = registrant.ServiceType;
                var implementationType = registrant.ImplementationType;
                if (implementationType == null) continue;

                if (IsIgnoreInjection(implementationType)) continue;

                var dependency = implementationType.GetCustomAttribute<DependencyAttribute>();
                var alreadyServiceDescriptor = result.FirstOrDefault(d => d.ServiceType == serviceType);
                if (dependency != null)
                {
                    if (alreadyServiceDescriptor != null)
                    {
                        var alreadyDependency = alreadyServiceDescriptor.ImplementationType?.GetCustomAttribute<DependencyAttribute>();
                        if (alreadyDependency?.ReplaceServices == true)
                            continue;

                        if (dependency.ReplaceServices || alreadyDependency?.TryRegister == true)
                            result.Remove(alreadyServiceDescriptor);
                        else if (dependency.TryRegister)
                            continue;
                    }
                    else
                    {
                        if (alreadyServiceDescriptor != null)
                        {
                            var alreadyDependency = alreadyServiceDescriptor.ImplementationType?.GetCustomAttribute<DependencyAttribute>();
                            if (alreadyDependency?.ReplaceServices == true)
                                continue;
                        }
                    }
                }
                result.Add(registrant);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsIgnoreInjection(Type type)
        {
            return type.GetTypeInfo().IsDefined(typeof(IgnoreInjectionAttribute));
        }

        private class ServiceDescriptorRegistrant
        {
            private ServiceDescriptorRegistrant(Type serviceType, ServiceLifetime lifetime)
            {
                Lifetime = lifetime;
                ServiceType = serviceType;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="serviceType"></param>
            /// <param name="implementationType"></param>
            /// <param name="lifetime"></param>
            public ServiceDescriptorRegistrant(
                Type serviceType,
                Type implementationType,
                ServiceLifetime lifetime)
                : this(serviceType, lifetime)
            {
                ImplementationType = implementationType;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="serviceType"></param>
            /// <param name="implementationType"></param>
            /// <param name="factory"></param>
            /// <param name="lifetime"></param>
            public ServiceDescriptorRegistrant(
                Type serviceType,
                Type implementationType,
                Func<IServiceProvider, object> factory,
                ServiceLifetime lifetime)
                : this(serviceType, lifetime)
            {
                ImplementationType = implementationType;
                ImplementationFactory = factory;
            }

            /// <summary>
            /// 
            /// </summary>
            public ServiceLifetime Lifetime { get; }

            /// <summary>
            /// 
            /// </summary>
            public Type ServiceType { get; }

            /// <summary>
            /// 
            /// </summary>
            public Type? ImplementationType { get; }

            /// <summary>
            /// 
            /// </summary>
            public Func<IServiceProvider, object>? ImplementationFactory { get; }
        }
    }
}
