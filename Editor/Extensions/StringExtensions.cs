using System.IO;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Extensions {
	public static class StringExtensions {

		/// <summary>
		/// Converts a Unity project relative path to system path
		/// </summary>
		/// <param name="path">Unity project relative path</param>
		/// <returns>System relative path</returns>
		public static string ToSystemPath(this string path) { return Application.dataPath.Replace("Assets", path); }

		/// <summary>
		/// Converts a system path to the relative to project path format used by Unity
		/// </summary>
		/// <param name="path">System path</param>
		/// <returns>Unity project relative path</returns>
		public static string ToUnityPath(this string path) { return path.Replace(Application.dataPath, "Assets"); }

		/// <summary>
		/// Removes an extension ( if present ) from a path
		/// </summary>
		/// <param name="path">The input path</param>
		/// <returns>The input path without the extension</returns>
		public static string RemoveExtension(this string path) { return path.Substring(0, path.Length - Path.GetFileName(path).Length); }

		/// <summary>
		/// Converts a GUID to Unity relative asset path
		/// </summary>
		/// <param name="guid">Asset GUID</param>
		/// <returns>Unity Relative Path</returns>
		public static string ToPath(this string guid) { return AssetDatabase.GUIDToAssetPath(guid); }

		/// <summary>
		/// Converts an Unity relative asset path to GUID
		/// </summary>
		/// <param name="path">Unity relative asset path</param>
		/// <returns>asset GUID</returns>
		public static string ToGUID(this string path) { return AssetDatabase.AssetPathToGUID(path); }
	}
}
