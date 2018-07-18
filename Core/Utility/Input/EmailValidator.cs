using System;
using System.Net.Mail;
using UnityEngine;

namespace Elarion.Utility.Input {
    [CreateAssetMenu(menuName = "Utils/Input/Email Validator", order = 51)]
    public class EmailValidator : InputValidator {
        protected override bool ValidateInputImpl(string input, out string error) {
            try {
                new MailAddress(input);
                error = null;
                return true;
            } catch(Exception e) {
                error = this.error;
                return false;
            }
        }
    }
}