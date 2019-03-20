using System;
using System.Net.Mail;
using UnityEngine;

namespace Elarion.Utility.Validators {
    [CreateAssetMenu(menuName = "Utils/Input/Email Validator", order = 51)]
    public class EmailValidator : InputValidator {
        protected override bool ValidateInputImpl(string input, out string error) {
            try {
                new MailAddress(input);
                error = null;
                return true;
            } catch(Exception) {
                error = this.error;
                return false;
            }
        }
    }
}