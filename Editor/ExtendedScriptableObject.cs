using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {

	[Serializable]
	public class ExtendedScriptableObject : ScriptableObject {
		public static T Save<T>(string path = "") where T : ExtendedScriptableObject {
			if(path == "")
				path = "Assets/" + typeof(T) + ".asset";
			var savedObject = CreateInstance(typeof(T));
			Directory.CreateDirectory(path.ToSystemPath().RemoveExtension());
			var savePath = AssetDatabase.GenerateUniqueAssetPath(path);
			AssetDatabase.CreateAsset(savedObject, savePath);
			Selection.activeObject = savedObject;
			return savedObject as T;
		}

	}

}