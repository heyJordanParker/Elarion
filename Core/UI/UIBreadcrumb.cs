using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Text))]
    public class UIBreadcrumb : BaseUIBehaviour {

        public UIComponent startingElement;
        public string elementSeparator = " - ";

        protected Text text;

        protected string Breadcrumb {
            get { return text.text; }
            set { text.text = value; }
        }

        protected override void Awake() {
            base.Awake();
            text = GetComponent<Text>();
            
            UpdateBreadcrumb();
        }

        protected virtual void UpdateBreadcrumb() {
            if(startingElement == null) {
                startingElement = GetComponent<UIComponent>();
                if(startingElement == null) {
                    return;
                }
            }

            if(!text) {
                text = GetComponent<Text>();
            }

            var parentComponent = startingElement;

            Breadcrumb = string.Empty;

            do {
                Breadcrumb = parentComponent.name + elementSeparator + Breadcrumb;

                parentComponent = parentComponent.Parent as UIComponent;
            } while(parentComponent != null);

            Breadcrumb = Breadcrumb.TrimEnd(elementSeparator.ToCharArray());
        }

        protected override void OnTransformParentChanged() {
            base.OnTransformParentChanged();

            UpdateBreadcrumb();
        }

        protected override void OnValidate() {
            base.OnValidate();
            
            UpdateBreadcrumb();
        }
    }
}