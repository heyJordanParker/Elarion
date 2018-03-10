using System.Linq;
using UnityEngine;

namespace Elarion.Singleton {
    public abstract class SingletonNew<TSingleton> : ScriptableObject where TSingleton : ScriptableObject, new() {

        // make sure the updater class has the DontDestroyOnLoad flag (make it optional) 
        // var executor = ExecutorBehavior.Create(GetType().name + " Singleton", true, HideFlags.HideAndDontSave);
        
        // check which scriptable object magic methods are already called and when (I don't want OnEnable getting called in the editor)
        
        public static TSingleton Instance {
            get { return Initializer<TSingleton>.LazyInstance; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Initializer<T> where T : ScriptableObject, new() {

            static Initializer() { }

            internal static readonly T LazyInstance = CreateInstance();

            private static T CreateInstance() {
                var instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                
                return instance ? instance : CreateInstance<T>();
            }
            
        }
    }
}