using System.Linq;
using Elarion.Extensions;
using Elarion.UI.Animation;
using Microsoft.Win32;
using UnityEngine;

namespace Elarion.UI {
    
    // TODO loading scene - an intermediary scene that opens another scene after a preset condition is met
    
    public class UIScene : UIPanel {
        
        // TODO Popup prefabs - create a default one (and auto load it when instantiating via EditorMenus) and leave it as a public field so users can change it with their own; Back functionality for popups (to close)
            
        // TODO snapping scroll for android-homescreen animations
        
        // TODO selected scene boolean; add custom editor showing the selected scene below the boolean (if it isn't the current scene)

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

        protected override void OpenInternal(bool skipAnimation, UIAnimation overrideAnimation) {
            transform.SetAsLastSibling();
            
            base.OpenInternal(skipAnimation, overrideAnimation);
        }

        // TODO move the hierarchy validation to the Editor.Update method (via [InitializeOnLoad] script); or any other method that'll run when the hierarchy updates
        protected override void OnValidate() {
            base.OnValidate();
            var uiRoot = FindObjectsOfType<UIRoot>().SingleOrDefault(root => root.transform.IsParentOf(transform));

            if(uiRoot != null) {
                foreach(var scene in uiRoot.GetComponentsInChildren<UIScene>(true)) {
                    if(scene == this) continue;
                    if(!scene.transform.IsParentOf(transform)) continue;

                    transform.SetParent(scene.transform.parent, false);
                    Debug.Log("Nesting scenes is not allowed. Removing nesting.", gameObject);
                    break;
                }
            }
                
            var rectTransform = transform as RectTransform;
            
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}