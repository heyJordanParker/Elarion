using System;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
	public class EGUI {
		public static readonly Action<Action> Horizontal = draw => {
			GUILayout.BeginHorizontal();
			draw();
			GUILayout.EndHorizontal();
		};
		 
		public static readonly Action<Action> Vertical = draw => {
			GUILayout.BeginVertical();
			draw();
			GUILayout.EndVertical();
		};

		public static void Space(uint amount = 0) { for(int i = 0; i < amount; ++i) EditorGUILayout.Space(); }
	}
}
