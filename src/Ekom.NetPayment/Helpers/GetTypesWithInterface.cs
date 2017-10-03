using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Umbraco.NetPayment.Helpers
{
    static class TypeLoaderExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }

    static class TypeHelper
    {
        public static IEnumerable<Type> GetTypesWithInterface(Assembly asm, Type myInterface)
        {
            return asm.GetLoadableTypes().Where(myInterface.IsAssignableFrom).ToList();
        }
    }
}
