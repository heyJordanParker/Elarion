using UnityEngine;

namespace Elarion.Attributes {
    public class EnumMultipleDropdownAttribute : PropertyAttribute {
        public string name;

        public EnumMultipleDropdownAttribute(string name = null) {
            this.name = name;
        }

    }
}
