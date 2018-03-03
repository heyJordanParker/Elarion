using System.Collections.Generic;
using Elarion.Attributes;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.UI {
    
    // TODO loading condition helper - a helper that switches the scene after a condition returns true (use unity events to hook up)
    
    public class UIScene : UIPanel {
        
        // TODO snapping scroll for android-homescreen animations
        
        // TODO selected scene boolean; add custom editor showing the selected scene below the boolean (if it isn't the current scene)

        [SerializeField, ReadOnly]
        private bool _initialScene = false;
        
        public bool InitialScene {
            get { return _initialScene; }
            set { _initialScene = value; }
        }
        
        // override this to ignore the ActiveChild flag
        public override bool ShouldRender {
            get { return Opened || InTransition; }
        }

        public override void Open(bool skipAnimation = false, UIAnimation overrideAnimation = null, bool focus = true, bool enable = true) {
            if(UIRoot.CurrentScene != this) {
                UIRoot.OpenScene(this, skipAnimation, overrideAnimation);
                return;
            }
            base.Open(skipAnimation, overrideAnimation, focus);
        }

        protected override void PreOpen() {
            base.PreOpen();
            transform.SetAsLastSibling();
        }

        protected override void OnValidate() {
            base.OnValidate();

            InitialScene = UIRoot.initialScene == this;

            // set the initial scene (restore when I move the initial scene here)
//            var initialScene = InitialScene ? this : Scenes.FirstOrDefault(s => s.InitialScene);
//            
//            if(initialScene == null) {
//                initialScene = Scenes.FirstOrDefault();
//                    
//                if(initialScene != null) {
//                    initialScene.InitialScene = true;
//                }
//            }
//                
//            foreach(var scene in Scenes) {
//                if(scene == initialScene) {
//                    continue;
//                }
//                
//                if(initialScene != null && scene.InitialScene) {
//                    scene.InitialScene = false;
//                }
//            }
        }

        private static List<UIScene> _scenes;

        public static List<UIScene> Scenes {
            get {
                if(_scenes == null) {
                    _scenes = SceneTools.FindSceneObjectsOfType<UIScene>();
                }

                return _scenes;
            }
        }
    }
}