using System.Linq;
using System.Reflection;
using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {

	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskDrawer : PropertyDrawer {

		private TypeAttribute MaskAttribute { get { return ((TypeAttribute)attribute); } }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return base.GetPropertyHeight(property, label); }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			int enumValueIndex = EditorGUI.MaskField(position, label, property.intValue, EnumerationNames);
			if(EditorGUI.EndChangeCheck()) property.intValue = enumValueIndex;
			EditorGUI.EndProperty();
		}

		public string[] EnumerationNames { 
			get { return MaskAttribute.type.GetFields(BindingFlags.Public | BindingFlags.Static).Select(field => field.Name).ToArray(); }
		}

	}
}
