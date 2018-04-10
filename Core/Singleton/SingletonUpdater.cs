using System.Collections.Generic;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.Singleton {
    
    // TODO add documentation

    // TODO open source

    // TODO release as a free asset on the asset store
    
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
    }
}