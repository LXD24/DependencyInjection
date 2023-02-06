using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    /// <summary>
    /// 忽略注入
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Interface)]
    public class IgnoreInjectionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public IgnoreInjectionAttribute()
        {
        }
    }
}
