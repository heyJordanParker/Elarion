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

        [SerializeField]
        private UIComponent _firstFocused;

        // override this to ignore the ActiveChild flag
        public override bool ShouldRender {
            get { return Opened || InTransition; }
        }

        protected override void OpenInternal(bool resetToSavedProperties, bool skipAnimation, UIAnimation overrideAnimation, bool autoEnable) {
            transform.SetAsLastSibling();
            
            base.OpenInternal(resetToSavedProperties, skipAnimation, overrideAnimation, autoEnable);
        }

        protected override void AfterOpen() {
            base.AfterOpen();
            
            if(_firstFocused != null) {
                _firstFocused.Focus();
            } else {
                Focus();
            }
        }

        // TODO move the hierarchy validation to the Editor.Update method (via [InitializeOnLoad] script); or any other method that'll run when the hierarchy updates
        protected override void OnValidate() {
            base.OnValidate();
            
            if(_firstFocused != null) {
                if(!_firstFocused.transform.IsChildOf(transform) || _firstFocused.gameObject == gameObject) {
                    _firstFocused = null;
                }
            }

            if(_firstFocused == null) {
                foreach(var component in gameObject.GetComponentsInChildren<UIComponent>()) {
                    if(component != this) {
                        _firstFocused = component;
                        break;
                    }
                }
            }

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