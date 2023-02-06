using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServiceDescriptorProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<ServiceDescriptor> GetServiceDescriptors();
    }
}
