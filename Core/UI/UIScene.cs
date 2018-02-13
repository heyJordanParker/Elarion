using Elarion.Extensions;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO basic loading scene - an intermediary scene that opens another scene after a preset condition is met
    
    public class UIScene : UIPanel {
        
        // TODO Popup prefabs - create a default one (and auto load it when instantiating via EditorMenus) and leave it as a public field so users can change it with their own; Back functionality for popups (to close)
            
        // TODO Button, panel, etc prefabs similar to the popup prefab; add editor menus for all of those (and *not* the base element) so the user can easily setup a UI
        
        // TODO snapping scroll for android-homescreen animations

        protected override bool OpenOnEnable {
            get { return false; }
        }
        
        // override open - automatically close all other scenes when opening and focus this while opening (clear this when the animation ends)
        
        protected override void Awake() {
            base.Awake();
//            canvas.enabled = false;
        }

        protected override void OpenInternal(bool resetToSavedProperties, bool skipAnimation, UIAnimation overrideAnimation, bool autoEnable) {
            base.OpenInternal(resetToSavedProperties, skipAnimation, overrideAnimation, autoEnable);
            ThisFocused = true;
        }
        // TODO When opening set this to the CurrentScene and close all other opened scenes in the parent (UIRoot)

        protected override void OnValidate() {
            base.OnValidate();

            var rectTransform = transform as RectTransform;
            
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            // TODO find the UIRoot in the parent hierarchy and make sure it's set to the Parent property; if it isn't - change the transform's parent to the UIRoot
            
        }
    }
}