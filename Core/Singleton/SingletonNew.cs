using System.Linq;
using UnityEngine;

namespace Elarion.Singleton {
    public abstract class SingletonNew<TSingleton> : ScriptableObject where TSingleton : ScriptableObject, new() {

        public static TSingleton Instance {
            get { return Initializer<TSingleton>.LazyInstance; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Initializer<T> where T : ScriptableObject, new() {

            static Initializer() { }

            internal static readonly T LazyInstance = CreateInstance<T>();

            private static T CreateInstance() {
                var instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                
                return instance ? instance : CreateInstance<T>();
            }
            
        }
    }
}