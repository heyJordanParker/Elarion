using System;
using System.Collections.Generic;
using System.IO;
using Elarion;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {

//	[InitializeOnLoad]
	public static class Hierarchy {

		private static DateTime _lastClick;
		private static int _clickCount;
		private static Vector2 _clickPosition;

		static Hierarchy() {
//			EditorApplication.projectWindowItemOnGUI += OnDoubleClick;
//			_lastClick = DateTime.Now;
//			_clickCount = 0;
//			_clickPosition = Vector2.zero;
		}

		static void OnDoubleClick(string guid, Rect rect) {
			var now = DateTime.Now;
			if((now - _lastClick) > new TimeSpan(0, 0, 0, 0, 500)) _clickCount = 0;
			if(!Event.current.isMouse || Event.current.type != EventType.mouseDown || !rect.Contains(Event.current.mousePosition)) return;
			if(!rect.Contains(_clickPosition)) _clickCount = 1;
			else ++_clickCount;
			_clickPosition = Event.current.mousePosition;
			if((now - _lastClick) < new TimeSpan(0, 0, 0, 0, 500) && _clickCount >= 2) {
				_clickCount = 0;
				_clickPosition = Vector2.zero;
				var assetPath = guid.ToPath();
				switch(Path.GetExtension(assetPath)) {
					case "":
						LoadFirstSubdirectoryAsset(guid, assetPath);
						Event.current.Use();
						break;
				}
			} else {
				_lastClick = now;	
			}
		}

		static void LoadFirstSubdirectoryAsset(string guid, string assetPath) {
			var subdirs = Directory.GetFileSystemEntries(assetPath);
			if(subdirs.Length <= 0) return;
			var childDirectories = new List<string>();
			for(int i = 0, count = subdirs.Length; i < count; ++i) {
				if(Path.GetExtension(subdirs[i]) == ".meta") continue;
				subdirs[i] = subdirs[i].Replace("\\", "/");
				childDirectories.Add(subdirs[i]);
			}
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(childDirectories[0].ToUnityPath());
		}

	}

}