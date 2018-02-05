using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    public class UIElement : UIComponent {

        protected Graphic graphic;
        
        public override float Alpha {
            get { return graphic.color.a; }
            set {
                var color = graphic.color;
                color.a = Mathf.Clamp01(value);
                graphic.color = color;
            }
        }

        protected override Behaviour Render {
            get { return graphic; }
        }

        protected override void Awake() {
            base.Awake();
            graphic = GetComponent<Graphic>();
            if(!graphic) {
                Debug.LogWarning("Cannot initialize a UIElement without a graphic component. Disablng.", gameObject);
                gameObject.SetActive(false);
                return;
            }
        }

        protected override void OnValidate() {
            var selectable = GetComponent<Selectable>();
            if(selectable) {
                selectable.interactable = interactable;
            }
        }
    }
}