using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModuleAttribute : Attribute
    {
        public string Alias { get; private set; }

        public ModuleAttribute(string alias)
        {
            Alias = alias;
        }

        private static Dictionary<string, IModule> _modules;

        public static IModule Find(string alias)
        {
            if (_modules == null)
            {
                _modules = new Dictionary<string, IModule>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
                {
                    var attr = type.GetCustomAttributes(typeof(ModuleAttribute), true).SingleOrDefault() as ModuleAttribute;
                    if (attr != null)
                    {
                        var instance = (IModule)Activator.CreateInstance(type);
                        _modules.Add(attr.Alias, instance);
                    }
                }
            }

            return _modules[alias];
        }
    }
}