using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.UI.Helpers.Animation;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.UI {
    
    // TODO loading scene helper - a helper that switches the scene after a condition returns true (use unity events or delegates to hook up); auto-close scene and open another one when loading finishes
    // TODO scene loading property that returns the helper or null
    // TODO loading indicator ui element - fetches the helper and sets a slider to 0-1 value based on the helper (1 if the helper is null); not exclusive to scenes
    
    public sealed class UIScene : UIPanel {
        
        // TODO snapping scroll for android-homescreen animations
        
        // TODO selected scene boolean; add custom editor showing the selected scene below the boolean (if it isn't the current scene)
        
        // TODO move the currentScene logic here

        [SerializeField]
        private bool _initialScene = false;
        
        public bool InitialScene {
            get { return _initialScene; }
            set { _initialScene = value; }
        }
        
        // override this to ignore the ActiveChild flag
        public override bool IsRendering {
            get { return State.IsOpened || State.IsInTransition; }
        }
        
        protected override void Start() {
            base.Start();

            if(InitialScene) {
                Open();
            }
        }

        protected override void BeforeOpen(bool skipAnimation) {
            base.BeforeOpen(skipAnimation);
            
            if(_currentScene != null) {
                _currentScene.Close(skipAnimation);
            }

            _currentScene = this;
            
            transform.SetAsLastSibling();
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            var allScenes = SceneTools.FindSceneObjectsOfType<UIScene>(); 

            var initialScene = InitialScene ? this : allScenes.FirstOrDefault(s => s.InitialScene);
            
            if(initialScene == null) {
                initialScene = allScenes.FirstOrDefault(s => s != this);
                    
                if(initialScene != null) {
                    initialScene.InitialScene = true;
                }
            }
                
            foreach(var scene in allScenes) {
                if(scene == initialScene) {
                    continue;
                }
                
                if(initialScene != null && scene.InitialScene) {
                    scene.InitialScene = false;
                }
            }
        }
#endif

        private static UIScene _currentScene;
        
        public static UIScene CurrentScene {
            get { return _currentScene; }
        }
    }
}