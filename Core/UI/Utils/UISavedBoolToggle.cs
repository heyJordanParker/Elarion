using Elarion.Saved.Variables;
using UnityEngine;

namespace Elarion.UI.Utils {
    /// <summary>
    /// A helper class that opens/closes a UI panel based on a bool value
    /// </summary>
    [RequireComponent(typeof(UIComponent))]
    public class UISavedBoolToggle : BaseUIBehaviour {

        public SavedBool toggle;

        [Tooltip("Should the open/close interaction get inverted.")]
        public bool inverted;

        private UIComponent _component;

        protected override void Awake() {
            base.Awake();
            _component = GetComponent<UIComponent>();
        }

        protected override void OnEnable() {
            toggle.Subscribe(OnToggleChanged);
        }

        protected override void OnDisable() {
            toggle.Unsubscribe(OnToggleChanged);
        }

        private void OnToggleChanged(bool value) {
            if(value) {
                _component.Toggle(!inverted);
            } else {
                _component.Toggle(inverted);
            }
        }

    }
}