using Elarion.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Workflows.Variables.Setters {
    [RequireComponent(typeof(Text))]
    public class TextSetter : TextSetterBase {
        [SerializeField, GetComponent]
        private Text _text;

        protected override string Text {
            get => _text.text;
            set => _text.text = value;
        }
    }
}