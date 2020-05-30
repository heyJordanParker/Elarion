using Elarion;
using Elarion.Attributes;
using Elarion.DataBinding.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI.Utils {
    [RequireComponent(typeof(Toggle))]
    public class SaveToggleValue : ExtendedBehaviour {

        [SerializeField, GetComponent]
        private Toggle _toggle;

        [SerializeField]
        private SavedBool _savedValue;

        private void OnEnable() {
            _toggle.isOn = _savedValue;
            
            _toggle.onValueChanged.AddListener(OnToggleChanged);
            _savedValue.Subscribe(OnSavedValueChanged);
        }

        private void OnSavedValueChanged(bool newValue) {
            _toggle.isOn = newValue;
        }

        private void OnToggleChanged(bool state) {
            _savedValue.Value = state;
        }
    }
}