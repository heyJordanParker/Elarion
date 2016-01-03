using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {

	public class LockTarget : UnityEditor.Editor {

		public const HideFlags Flags = HideFlags.NotEditable | HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		//HideInHierarchy - disables clicking the object

		[MenuItem("GameObject/Lock")]
		public static void Lock() {
			foreach(var go in Selection.activeGameObject.GetComponentsInChildren<Transform>().Select(child => child.gameObject)) go.hideFlags = Flags;
		}

		[MenuItem("GameObject/Lock", true)]
		public static bool ValidateLock() {
			if(Selection.activeGameObject == null) return false;
			return Selection.activeGameObject.hideFlags == 0;
		}

		[MenuItem("GameObject/Unlock")]
		public static void Unlock() {
			foreach(var go in Selection.activeGameObject.GetComponentsInChildren<Transform>().Select(child => child.gameObject)) go.hideFlags = 0;
		}

		[MenuItem("GameObject/Unlock", true)]
		public static bool ValidateUnlock() {
			if(Selection.activeGameObject == null) return false;
			return Selection.activeGameObject.hideFlags != 0;
		}

		[MenuItem("GameObject/Toggle Lock %#w")]
		public static void LockUnlock() {
			if(ValidateLock()) Lock();
			else if(ValidateUnlock()) Unlock();
		}

	}

}