using System;
using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
	[CustomPropertyDrawer(typeof(EnumAttribute))]
	public class EnumDrawer : PropertyDrawer {

		private TypeAttribute EnumAttribute { get { return ((TypeAttribute)attribute); } }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return base.GetPropertyHeight(property, label); }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			int enumValueIndex = EditorGUI.Popup(position, EnumAttribute.type.Name, property.intValue, EnumerationNames);
			if(EditorGUI.EndChangeCheck()) property.intValue = enumValueIndex;
			EditorGUI.EndProperty();
		}

		public string[] EnumerationNames {
			get { return Enum.GetNames(EnumAttribute.type); }
		}

	}
}