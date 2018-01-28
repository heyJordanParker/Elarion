using Elarion.Extensions;
using Elarion.UI.Animation;
using Elarion.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO make this a generic fullscreen element
    // TODO Move the canvas to the fullscreen element; they need to display properly over other things 
    
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIScreen : UIElement {
        protected override void Awake() {
            base.Awake();
            // TODO make sure this is fullscreen?
            
            // TODO register this as a fullscreen element? (to handle blurring on popups etc)
            
            // TODO make sure it's not a top-level canvas. Animations might not work with them. Validate this. 
        }

        protected override void UpdateState() {
            base.UpdateState();

            if(!Active) {
                return;
            }

            if(InTransition) {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
            } else {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }

        protected override void OnOpen() {
            base.OnOpen();
            Fullscreen = true;
        }

        protected override void OnClose() {

            base.OnClose();
            Fullscreen = false;
        }
    }
}