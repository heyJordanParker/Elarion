using Elarion.Extensions;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    public class UIScene : UIPanel {
        
        // maybe make this a generic UIElement
        
        // TODO Popup prefabs - create a default one (and auto load it when instantiating via EditorMenus) and leave it as a public field so users can change it with their own
            
        // TODO Button, panel, etc prefabs similar to the popup prefab; add editor menus for all of those (and *not* the base element) so the user can easily setup a UI
        
        // TODO snapping scroll for android-homescreen animations
        
        // Scene Name? (for debugging)

        protected override void Awake() {
            base.Awake();
            Fullscreen = true;
        }

        protected override void OnValidate() {
            // TODO figure out how to integrate global panels/elements (e.g. popups) into the approach below
            
            // TODO make sure there aren't any other scenes above this (or even any UIHierarchy objects, besides the UIRoot)
            
            // TODO make sure this is below a UIRoot; runtime disable if not
            
        }
    }
}