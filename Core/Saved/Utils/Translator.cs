using System;
using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Saved.Variables.References;
using Elarion.Utility.Tuples;
using UnityEngine;

namespace Elarion.Saved.Utils {
    // TODO a localizer class that associates a language code with translators and can resolve words as well  
    [CreateAssetMenu(menuName = "Utils/Translator", order = 51)]
    public class Translator : ScriptableObject {

        [SerializeField]
        private StringReference _string;

        [SerializeField]
        private List<StringTuple> _translations;

        [SerializeField]
        private bool _hasDefaultValue;

        [SerializeField, ConditionalVisibility("_hasDefaultValue")]
        private string _defaultValue = "Default Value";

        public string Value {
            get {
                var stringValue = _string == null ? string.Empty : _string.Value;
                var translation = _translations.SingleOrDefault(tuple => tuple.Key == stringValue);
                if(translation != null) {
                    return translation.Value;
                }

                if(_hasDefaultValue) {
                    return _defaultValue;
                }

                return stringValue;
            }
        } 

        private void Reset() {
            _translations = new List<StringTuple> {
                new StringTuple {Key = "Example Value 1", Value = "Returned Translation 1"},
                new StringTuple {Key = "Example Value 2", Value = "Returned Translation 2"}
            };
        }
                
        public static implicit operator string(Translator translator) {
            return translator == null ? string.Empty : translator.Value;
        }
    }
}