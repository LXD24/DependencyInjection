using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    /// <summary>
    /// 程序集加载器
    /// </summary>
    public interface IAssemblyLoader
    {
        Assembly[] LoadAssemblies();
    }
}
