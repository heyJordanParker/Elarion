using UnityEngine;

namespace Elarion.Attributes {
    public class StringDropdownAttribute : PropertyAttribute {
        public string[] choices;
        public string name;

        /// <summary>
        /// Pick a value from a list of strings
        /// </summary>
        /// <param name="choices">List of valid choices</param>
        /// <param name="name">Name in the inspector</param>
        public StringDropdownAttribute(string[] choices, string name = null) {
            this.choices = choices;
            this.name = name;
        }
    }
}