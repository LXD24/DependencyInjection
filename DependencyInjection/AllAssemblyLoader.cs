using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DependencyInjection
{
    /// <summary>
    /// 程序集加载器
    /// </summary>
    internal class AllAssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// 加载程序集
        /// </summary>
        public Assembly[] LoadAssemblies()
        {
            var context = DependencyContext.Default;
            var assemblyNames = LoadAllAssemblyNames();
            if (context != null && !assemblyNames.Any())
            {
                foreach (var library in context.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package"))
                {
                    var name = library.Name;
                    if (!assemblyNames.Contains(name))
                        assemblyNames.Add(name);
                }
            }

            return LoadAssemblies(assemblyNames);
        }

        /// <summary>
        /// 加载所有程序集名称
        /// </summary>
        /// <returns></returns>
        private List<string> LoadAllAssemblyNames()
        {
            var context = DependencyContext.Default;
            var result = new List<string>();
            string[] assemblyNames = null;
            if (context != null)
            {
                assemblyNames = context
                    .CompileLibraries
                    .Where(lib => !lib.Serviceable && lib.Type != "package")
                    .SelectMany(m => m.Assemblies)
                    .Distinct()
                    .Select(m => m.Replace(".dll", ""))
                    .OrderBy(m => m).ToArray();
            }
            if (assemblyNames?.Length > 0)
            {
                result =
                    assemblyNames.Select(name =>
                        {
                            var i = name.LastIndexOf('/') + 1;
                            return name.Substring(i, name.Length - i);
                        })
                        .Distinct()
                        .OrderBy(m => m)
                        .ToList();
            }
            return result;
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="files">文件集合</param>
        protected Assembly[] LoadAssemblies(IEnumerable<string> files)
        {
            var assemblies = new List<Assembly>();
            foreach (var file in files)
            {
                var name = new AssemblyName(file);
                try
                {
                    assemblies.Add(Assembly.Load(name));
                }
                catch (FileNotFoundException)
                {
                }
            }
            return assemblies.ToArray();
        }
    }
}
