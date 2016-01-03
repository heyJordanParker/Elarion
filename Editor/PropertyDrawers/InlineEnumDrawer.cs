using System;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
	public class InlineEnumDrawer<TEnum> : PropertyDrawer {

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return base.GetPropertyHeight(property, label) * Stats.Length; }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			if(property.arraySize != Stats.Length) {
				InitStat(property);
				return;
			}
			position.height /= Stats.Length;
			foreach(var box in property) {
				var stat = (SerializedProperty)box;
				stat.FindPropertyRelative("value").floatValue = EditorGUI.FloatField(position,
																	stat.FindPropertyRelative("name").stringValue,
																	stat.FindPropertyRelative("value").floatValue);
				position.y += base.GetPropertyHeight(property, label);
			}

			EditorGUI.EndProperty();
		}

		private void InitStat(SerializedProperty property) {
			while(property.arraySize > Stats.Length) {
				property.DeleteArrayElementAtIndex(property.arraySize - 1);
			}
			while(property.arraySize < Stats.Length) {
				int i = property.arraySize;
				property.InsertArrayElementAtIndex(i);
				var stat = property.GetArrayElementAtIndex(i);
				stat.FindPropertyRelative("name").stringValue = Names[i];
				stat.FindPropertyRelative("value").floatValue = 0;
			}
		}

		public string[] Names {
			get { return Enum.GetNames(typeof(TEnum)); }
		}

		public TEnum[] Stats {
			get { return Enum.GetValues(typeof(TEnum)) as TEnum[]; }
		}

	}
}