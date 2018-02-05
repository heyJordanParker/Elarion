using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elarion.UI {
    public class UIElement : UIComponent {

        [SerializeField]
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
            
            if(!graphic) {
                graphic = GetComponent<Graphic>();

                if(graphic) {
                    return;
                }
                
                Debug.LogWarning("Cannot initialize a UIElement without a graphic component. Disablng.", gameObject);
                gameObject.SetActive(false);
            }
        }

        protected override void OnValidate() {
            base.OnValidate();
            var selectable = GetComponent<Selectable>();
            if(selectable) {
                selectable.interactable = interactable;
            }
        }
    }
}