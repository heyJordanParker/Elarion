using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.UI.Helpers.Animation;
using Elarion.Utility;
using UnityEngine;

namespace Elarion.UI {
    
    // TODO loading condition helper - a helper that switches the scene after a condition returns true (use unity events to hook up)
    
    public class UIScene : UIPanel {
        
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

        protected override void Awake() {
            base.Awake();

            if(!AllScenes.Contains(this)) {
                AllScenes.Add(this);
            }
        }
        
        protected override void Start() {
            base.Start();

            if(InitialScene) {
                Open();
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            
            AllScenes.Remove(this);
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

            var initialScene = InitialScene ? this : AllScenes.FirstOrDefault(s => s.InitialScene);
            
            if(initialScene == null) {
                initialScene = AllScenes.FirstOrDefault(s => s != this);
                    
                if(initialScene != null) {
                    initialScene.InitialScene = true;
                }
            }
                
            foreach(var scene in AllScenes) {
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

        private static readonly List<UIScene> Scenes = new List<UIScene>();

        public static List<UIScene> AllScenes {
            get {
                if(Scenes.Count == 0) {
                    Scenes.AddRange(SceneTools.FindSceneObjectsOfType<UIScene>());
                }

                return Scenes;
            }
        }
    }
}