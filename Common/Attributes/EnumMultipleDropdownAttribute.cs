using UnityEngine;

namespace Elarion.Common.Attributes {
    public class EnumMultipleDropdownAttribute : PropertyAttribute {
        public readonly string name;

        public EnumMultipleDropdownAttribute(string name = null) {
            this.name = name;
        }

    }
}
