using System.Collections.Generic;
using System.Linq;
using Elarion.Attributes;
using Elarion.Tuples;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Transformers {
    // TODO a localizer class that associates a language code with translators and can resolve words as well  
    public class TranslationTransformer : ExtendedBehaviour {

        [SerializeField]
        private StringReference _string;

        [SerializeField]
        private List<StringTuple> _translations = new List<StringTuple> {
            new StringTuple {Key = "Example Value 1", Value = "Returned Translation 1"},
            new StringTuple {Key = "Example Value 2", Value = "Returned Translation 2"}
        };

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
                
        public static implicit operator string(TranslationTransformer translator) {
            return translator == null ? string.Empty : translator.Value;
        }
    }
}