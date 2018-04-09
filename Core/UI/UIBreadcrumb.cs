using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    [RequireComponent(typeof(Text))]
    public class UIBreadcrumb : BaseUIBehaviour {

        public string elementSeparator = " - ";

        protected Text text;

        private UIComponent _focusedComponent;

        protected string Breadcrumb {
            get { return text.text; }
            set { text.text = value; }
        }

        protected override void Awake() {
            base.Awake();
            text = GetComponent<Text>();
        }

        private void Update() {
            if(_focusedComponent == UIFocusableComponent.FocusedComponent) {
                return;
            }

            var newFocusedComponent = UIFocusableComponent.FocusedComponent != null ? UIFocusableComponent.FocusedComponent : UIScene.CurrentScene;

            if(_focusedComponent == newFocusedComponent) {
                return;
            }
            
            _focusedComponent = newFocusedComponent;
            UpdateBreadcrumb();
        }

        private void UpdateBreadcrumb() {
            var parentComponent = _focusedComponent;

            Breadcrumb = string.Empty;

            while(parentComponent != null && !(parentComponent is UIScene)) {
                Breadcrumb = parentComponent.name + elementSeparator + Breadcrumb;

                parentComponent = parentComponent.ParentComponent;
            }

            Breadcrumb = UIScene.CurrentScene.name + elementSeparator + Breadcrumb;

            Breadcrumb = Breadcrumb.TrimEnd(elementSeparator.ToCharArray());
        }
    }
}