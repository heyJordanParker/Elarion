using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
	public class PlayerPrefsControl : UnityEditor.Editor {
 
		[MenuItem("Tools/Delete PlayerPrefs")] 
		public static void DeletePlayerPrefs() {
			PlayerPrefs.DeleteAll();
		} 

	}
}
