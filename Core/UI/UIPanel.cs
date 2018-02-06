using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))] // to propagate input events
    public class UIPanel : UIComponent {
        // TODO UIForm inheritor - add error checking submitting and so on builtin
        // TODO UIDialog inheritor - dynamic amount of (getcomponent; onchildren changed), extensible; dialog skins?
        
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
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        protected override void OnValidate() {
            base.OnValidate();
            var canvasGroup = GetComponent<CanvasGroup>();
            if(canvasGroup) {
                canvasGroup.interactable = interactable;
            }
        }
    }
}