using UnityEngine;

namespace Elarion.Attributes {
    public class EnumMultipleDropdownAttribute : PropertyAttribute {
        public readonly string name;

        public EnumMultipleDropdownAttribute(string name = null) {
            this.name = name;
        }

    }
}
