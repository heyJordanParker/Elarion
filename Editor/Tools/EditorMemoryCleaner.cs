using System;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Tools {
    public static class EditorMemoryCleaner {
        [MenuItem("Tools/Run Memory Cleaner", priority = 35)]
        static void CleanMemory() {
            EditorUtility.UnloadUnusedAssetsImmediate();
            GC.Collect();
            Debug.Log("Garbage collection and unloading of unused assets finished.");
        }
    }
}