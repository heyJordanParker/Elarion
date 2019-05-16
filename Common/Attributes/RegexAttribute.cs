using UnityEngine;

namespace Elarion.Common.Attributes {
    public class RegexAttribute : PropertyAttribute {
        public readonly string helpMessage;

        public readonly string pattern;

        public RegexAttribute(string pattern, string helpMessage) {
            this.pattern = pattern;
            this.helpMessage = helpMessage;
        }
    }
}