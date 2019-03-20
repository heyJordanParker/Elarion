using UnityEngine;

namespace Elarion.Utility.Validators {
    [CreateAssetMenu(menuName = "Utils/Input/Length Validator", order = 51)]
    public class LengthValidator : InputValidator {

        [Range(0, 14)]
        public int requiredLength = 6;

        protected override bool ValidateInputImpl(string input, out string error) {
            if(string.IsNullOrEmpty(input) || input.Length < requiredLength) {
                error = this.error;
                return false;
            }
            error = null;
            return true;
        }
    }
}