using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Singleton {
    public abstract class SingletonBehaviour : MonoBehaviour {
        
        private static Dictionary<Type, SingletonBehaviour> _instances;

        private static Dictionary<Type, SingletonBehaviour> Instances {
            get {
                if(_instances == null)
                    _instances = new Dictionary<Type, SingletonBehaviour>();
                return _instances;
            }
        }

        public static T Get<T>() where T : SingletonBehaviour {
            SingletonBehaviour singleton;
            if(!Instances.TryGetValue(typeof(T), out singleton)) {
                return null;
            }
            return (T) singleton;
        }

        protected virtual void Awake() {
            var type = GetType();
            SingletonBehaviour instance;
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

        
        protected virtual void OnValidate() {
#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.SceneManagement.EditorSceneManager.loadedSceneCount == 0) {
                // loaded scene count is to prevent GetActiveScene from throwing an exception when exiting play mode
                return;
            }
            
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            var rootGameObjects = scene.GetRootGameObjects();

            var singletons = new List<SingletonBehaviour> {this};


            foreach(var gameObject in rootGameObjects) {
                var component = gameObject.GetComponent(GetType());

                if(!component) {
                    component = gameObject.GetComponentInChildren(GetType(), true);
                }

                if(component == this) {
                    continue;
                }
                
                if(component != null) {
                    singletons.Add(component as SingletonBehaviour);
                }
            }

            if(singletons.Count > 1) {
                Debug.Log("The scene contains multiple instances of the " + GetType().Name + " Singleton.", this);
            }
#endif
        }
    }
}
