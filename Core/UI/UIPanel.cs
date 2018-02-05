using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO UIForm inheritor - add error checking submitting and so on builtin
        // TODO UIDialog inheritor - custom amount of buttons, extensible, based on prefab (so the user can skin it); dialog skins?
        
        protected Canvas canvas;
        protected CanvasGroup canvasGroup;
        
        public override float Alpha {
            get { return canvasGroup.alpha; }
            set { canvasGroup.alpha = Mathf.Clamp01(value); }
        }

        protected override Behaviour Render {
            get { return canvas; }
        }

        protected override void Awake() {
            base.Awake();
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
            
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        protected override void OnValidate() {
            var canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.hideFlags = HideFlags.NotEditable;
            if(canvasGroup) {
                canvasGroup.interactable = interactable;
            }
        }
    }
}