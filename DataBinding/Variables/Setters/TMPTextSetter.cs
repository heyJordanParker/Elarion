
using TMPro;
using UnityEngine;
#if TMP_ENABLED
using Elarion.Common.Attributes;

namespace Elarion.DataBinding.Variables.Setters {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTextSetter : TextSetterBase {
        [SerializeField, GetComponent]
        private TextMeshProUGUI _text;

        protected override void UpdateText() {
            _text.SetText(Text);
        }
    }
}
#endif
