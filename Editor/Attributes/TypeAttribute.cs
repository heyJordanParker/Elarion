using System;
using UnityEngine;

namespace Elarion.Attributes {
	public class TypeAttribute : PropertyAttribute {

		public readonly Type type;

		public TypeAttribute(Type t) {
			if(!t.IsEnum) throw new ArgumentException("EnumMaskAttribute should be used with an enumerated type");
			type = t;
		}

	}
}