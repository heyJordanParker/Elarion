using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Singleton {
    public abstract class Singleton : MonoBehaviour {
        
        // TODO finish the SingletonNew class & replace this with that
        
        private static Dictionary<Type, Singleton> _instances;

        private static Dictionary<Type, Singleton> Instances {
            get {
                if(_instances == null)
                    _instances = new Dictionary<Type, Singleton>();
                return _instances;
            }
        }

        public static T Get<T>() where T : Singleton {
            Singleton singleton;
            if(!Instances.TryGetValue(typeof(T), out singleton)) {
                return null;
            }
            return (T) singleton;
        }

        protected virtual void Awake() {
            var type = GetType();
            Singleton instance;
            if(!Instances.TryGetValue(type, out instance)) {
                Instances.Add(type, this);
            } else if(instance != this) {
                Debug.Log("Destroying Singleton of type " + type.Name + " in GameObject " + gameObject.name + " because an instance of this Singleton already exists.", gameObject);
                Destroy(this);
            }
        }

        protected void OnDestroy() {
            Instances.Remove(GetType());
        }


        public static void Cleanup() {
            _instances = null;
        }

        
        #if UNITY_EDITOR
        protected virtual void OnValidate() {
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.SceneManagement.EditorSceneManager.loadedSceneCount == 0) {
                // loaded scene count is to prevent GetActiveScene from throwing an exception when exiting play mode
                return;
            }
            
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            var rootGameObjects = scene.GetRootGameObjects();

            var singletons = new List<Singleton> {this};


            foreach(var gameObject in rootGameObjects) {
                var component = gameObject.GetComponent(GetType());

                if(!component) {
                    component = gameObject.GetComponentInChildren(GetType(), true);
                }

                if(component == this) {
                    continue;
                }
                
                if(component != null) {
                    singletons.Add(component as Singleton);
                }
            }

            if(singletons.Count > 1) {
                Debug.Log("The scene contains multiple instances of the " + GetType().Name + " Singleton.", this);
            }
        }
        #endif
    }
}
