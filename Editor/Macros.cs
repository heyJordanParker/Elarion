using UnityEditor;
using UnityEditor.Macros;
using UnityEngine;

namespace Elarion.Editor {

	public class Macros : EditorWindow {
		string _macro = "";

		[MenuItem("Tools/Macros")]
		static void Init() {
			CreateInstance<Macros>().ShowUtility();
		}

		void OnGUI() {
			_macro = EditorGUILayout.TextArea(_macro, GUILayout.ExpandHeight(true));

			if(GUILayout.Button("Execute")) {
				MacroEvaluator.Eval(_macro);
			}
		}
	}

}