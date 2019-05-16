using Elarion.DataBinding.Variables.References;
using UnityEngine;

namespace Elarion.Tools.InputValidators {
    [CreateAssetMenu(menuName = "Utils/Input/Matching Validator", order = 51)]
    public class MatchingValidator : InputValidator {

        [Tooltip("String reference to match the field's value with.")]
        public StringReference matchWith;
        
        protected override bool ValidateInputImpl(string input, out string error) {
            if(matchWith.Value != input) {
                error = this.error;
                return false;
            }

            error = null;
            return true;
        }
    }
}