using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace YSL.Host.InitConfig
{
    public static class ApiAssembie
    {
        public static void LoadApiAssembie(string apiAssembly)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + apiAssembly;
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(assembly => AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), assemblyName)))
            {
                AppDomain.CurrentDomain.Load(assemblyName);
            }
        }
        public static void LoadApiAssembie(ArrayList apiAssemblies)
        {
            foreach (var apiAssembly in apiAssemblies)
            {
                LoadApiAssembie(apiAssembly.ToString());
            }
        }
    }
}
