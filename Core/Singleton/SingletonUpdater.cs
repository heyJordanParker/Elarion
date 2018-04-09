using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.Singleton {
    public class SingletonUpdater : ExecutorBehavior {
        [SerializeField]
        private List<ScriptableObject> _singletons = new List<ScriptableObject>();

        private static SingletonUpdater _updater;

        private static SingletonUpdater Updater {
            get {
                if(_updater == null) {
                    _updater = Create<SingletonUpdater>("Singleton Updater", true); // , HideFlags.HideAndDontSave
                }

                return _updater;
            }
        }

        public static void RegisterSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : ScriptableObject, ISingleton {
            Updater.DestroyEvent += singleton.Deinitialize;

            Updater.FixedUpdateEvent += singleton.OnFixedUpdate;
            Updater.UpdateEvent += singleton.OnUpdate;
            Updater.LateUpdateEvent += singleton.OnLateUpdate;


            Updater.ApplicationFocusEvent += singleton.OnApplicationFocus;
            Updater.ApplicationPauseEvent += singleton.OnApplicationPause;
            Updater.ApplicationQuitEvent += singleton.OnApplicationQuit;

            Updater.DrawGizmosEvent += singleton.OnDrawGizmos;
            Updater.PostRenderEvent += singleton.OnPostRender;
            Updater.PreCullEvent += singleton.OnPreCull;
            Updater.PreRenderEvent += singleton.OnPreRender;
            Updater.ResetEvent += singleton.OnReset;

            Updater._singletons.Add(singleton);
        }

        public static void UnregisterSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : ScriptableObject, ISingleton {
            Updater.DestroyEvent -= singleton.Deinitialize;

            Updater.FixedUpdateEvent -= singleton.OnFixedUpdate;
            Updater.UpdateEvent -= singleton.OnUpdate;
            Updater.LateUpdateEvent -= singleton.OnLateUpdate;


            Updater.ApplicationFocusEvent -= singleton.OnApplicationFocus;
            Updater.ApplicationPauseEvent -= singleton.OnApplicationPause;
            Updater.ApplicationQuitEvent -= singleton.OnApplicationQuit;

            Updater.DrawGizmosEvent -= singleton.OnDrawGizmos;
            Updater.PostRenderEvent -= singleton.OnPostRender;
            Updater.PreCullEvent -= singleton.OnPreCull;
            Updater.PreRenderEvent -= singleton.OnPreRender;
            Updater.ResetEvent -= singleton.OnReset;

            Updater._singletons.Remove(singleton);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSingletons() {
            
            // TODO test this in a bigger project. I'm getting ~170ms on a simple project
            // TODO test this on my phone - maybe without the editor the performance will be much higher (set a static int in TestClass and render it on screen)

            var sw2 = new Stopwatch();
            
            sw2.Start();
            
            var singletons =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where !type.IsAbstract && !type.IsGenericType &&
                      type.GetCustomAttributes(typeof(SingletonAttribute), true).Length > 0
                select type;
            
            foreach(var singleton in singletons) {
                var method = singleton.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .FirstOrDefault(m =>
                        m.GetCustomAttributes(typeof(SingletonCreateInstanceAttribute), true).Length > 0);
                
                method.Invoke(null, null);
            }
//            
            sw2.Stop();

            TestClass.time = sw2.Elapsed.TotalMilliseconds;
//
//            Debug.Log("Total Time " + sw2.Elapsed.TotalMilliseconds);
        }

        // TODO add documentation

        // TODO open source

        // TODO release as a free asset on the asset store
    }
}