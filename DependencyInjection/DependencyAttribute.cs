using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    /// <summary>
    /// 需要和 ISingletonDependency、IScopedDependency、ITransientDependency 使用
    /// </summary>
    public class DependencyAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public DependencyAttribute()
        {
        }

        /// <summary>
        /// 设置 true 则仅当服务未注册时才会被注册
        /// </summary>
        public bool TryRegister { get; set; } = false;

        /// <summary>
        /// 设置 true 则替换之前已经注册过的服务
        /// </summary>
        public bool ReplaceServices { get; set; } = false;
    }
}
