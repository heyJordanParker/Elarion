using System;
using UnityEngine;

namespace Elarion.Common.Attributes {
    public class EnumDropdownAttribute : PropertyAttribute {
        public readonly Type type;
        public readonly string name;

        /// <summary>
        /// Pick a value from an enumerator
        /// </summary>
        /// <param name="enumType">Enumerator to pick a value from (or the property will be used)</param>
        /// <param name="name">Name in the inspector</param>
        public EnumDropdownAttribute(Type enumType, string name = null) {
            if(enumType != null) {
                if(!enumType.IsEnum) throw new ArgumentException("EnumDropdownAttribute needs an enum parameter");

                type = enumType;
            }
            this.name = name;
        }
    }
}