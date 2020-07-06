#if TMP_ENABLED

using Elarion.Attributes;
using TMPro;
using UnityEngine;

namespace Elarion.Workflows.Variables.Setters {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTextSetter : TextSetterBase {
        [SerializeField, GetComponent]
        private TextMeshProUGUI _text;
        
        protected override string Text {
            get => _text.text;
            set => _text.SetText(value);
        }
    }
}
#endif
