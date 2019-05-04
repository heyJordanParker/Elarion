using Elarion.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Saved.Variables.Setters {
    [RequireComponent(typeof(Text))]
    public class TextSetter : TextSetterBase {

        [SerializeField, GetComponent]
        private Text _text;

        protected override void UpdateText() {  
            _text.text = Text;
        
        }
    }
}