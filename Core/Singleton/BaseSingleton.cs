using System;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Elarion.Singleton {
    public interface ISingleton {
        void Initialize();
        void Deinitialize();

        void OnFixedUpdate();
        void OnUpdate();
        void OnLateUpdate();

        void OnApplicationFocus();
        void OnApplicationPause();
        void OnApplicationQuit();

        void OnDrawGizmos();
        void OnPostRender();
        void OnPreCull();
        void OnPreRender();
        void OnReset();
    }

    public abstract class LazySingleton<TSingleton> : BaseSingleton where TSingleton : BaseSingleton {
        public static TSingleton Instance {
            get { return Initializer<TSingleton>.LazyInstance; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Initializer<T> where T : BaseSingleton {
            static Initializer() { }

            internal static readonly T LazyInstance = Create<T>();
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    internal class SingletonAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Method)]
    internal class SingletonCreateInstanceAttribute : Attribute { }

    [Singleton]
    public abstract class Singleton<TSingleton> : BaseSingleton where TSingleton : BaseSingleton {
        public static TSingleton Instance { get; private set; }

        [SingletonCreateInstance]
        protected static void CreateInstance() {
            if(Instance != null) {
                return;
            }
            
            Instance = Create<TSingleton>();
        }
    }

    public abstract class BaseSingleton : ScriptableObject, ISingleton {
        public virtual void Initialize() {
            SingletonUpdater.RegisterSingleton(this);
        }

        public virtual void Deinitialize() {
            SingletonUpdater.UnregisterSingleton(this);
        }

        public virtual void OnFixedUpdate() { }
        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnApplicationFocus() { }
        public virtual void OnApplicationPause() { }
        public virtual void OnApplicationQuit() { }
        public virtual void OnDrawGizmos() { }
        public virtual void OnPostRender() { }
        public virtual void OnPreCull() { }
        public virtual void OnPreRender() { }
        public virtual void OnReset() { }
        
        protected virtual void OnValidate() {
#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);
            var separator = System.IO.Path.DirectorySeparatorChar;

            if(!path.Contains(separator + "Resources" + separator)) {
                Debug.LogWarning("All " + GetType().Name + " (Singleton) instances must be in the Resources folder. Otherwise they won't be loaded outside of the Editor.", this);
            }
#endif
        }

        protected static T Create<T>() where T : BaseSingleton, ISingleton {
            var instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();

            instance = instance ? instance : CreateInstance<T>();

            instance.Initialize();

            return instance;
        }
    }
}