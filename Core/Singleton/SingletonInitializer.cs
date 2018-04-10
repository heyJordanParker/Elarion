using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Elarion.Singleton {
    public static class SingletonInitializer {
        public static readonly string[] BuiltinAssemblies = {
            "Assembly-CSharp-Editor", "Assembly-CSharp-Editor-firstpass", "Boo.", "ExCSS.Unity", "Mono", "Mono.",
            "mscorlib", "nunit.", "System", "System.", "Unity.", "UnityEngine", "UnityEditor", "UnityScript"
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSingletons() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly =>
                !BuiltinAssemblies.Any(builtinAssemblyName => assembly.FullName.StartsWith(builtinAssemblyName)));

            var singletons =
                from assembly in assemblies
                from type in assembly.GetTypes()
                where !type.IsAbstract && !type.IsGenericType &&
                      type.GetCustomAttributes(typeof(SingletonAttribute), true).Length > 0
                select type;

            foreach(var singleton in singletons) {
                var method = singleton
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .FirstOrDefault(m =>
                        m.GetCustomAttributes(typeof(SingletonCreateInstanceAttribute), true).Length > 0);

                method.Invoke(null, null);
            }
        }
    }
}