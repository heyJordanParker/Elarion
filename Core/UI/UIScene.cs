using Elarion.Extensions;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    public class UIScene : UIPanel {
        
        // TODO register this as a fullscreen element? (to handle blurring on popups etc)
            
        // TODO Popup prefabs - create a default one (and auto load it when instantiating via EditorMenus) and leave it as a public field so users can change it with their own
            
        // TODO Button, panel, etc prefabs similar to the popup prefab; add editor menus for all of those (and *not* the base element) so the user can easily setup a UI
        
        // TODO snapping scroll for android-homescreen animations

        protected override void Awake() {
            base.Awake();
            Fullscreen = true;
        }

        protected override void OnValidate() {
            // TODO figure out how to integrate global panels/elements (e.g. popups) into the approach below
            
            // TODO make sure there aren't any other scenes above this
            
            // TODO make sure this isn't a top level canvas (if it is - create a new top level canvas, add a canvas scaler to it, make this a child element, and make this fullscreen; add this to another top-level canvas if it exists [find out by the object having a canvas scaler component and not having a UIScene component])
            
        }
    }
}