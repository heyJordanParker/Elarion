using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor {
    public static class AsyncProgressBar {
        static AsyncProgressBar() {
            var type = typeof(EditorUtility).Assembly.GetType("UnityEditor.AsyncProgressBar");
            if(type != null) {
                var displayMethod = type.GetMethod("Display", BindingFlags.Static | BindingFlags.Public);
                var clearMethod = type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
                if(displayMethod != null && clearMethod != null) {
                    Display =
                        Delegate.CreateDelegate(typeof(Action<string, float>), displayMethod) as Action<string, float>;
                    Clear = Delegate.CreateDelegate(typeof(Action), clearMethod) as Action;
                    return;
                }
            }

            Debug.LogError("Can't find async progress bar via Reflection. Using default one.");

            Display = (title, progress) => EditorUtility.DisplayProgressBar("Loading", title, progress);
            Clear = EditorUtility.ClearProgressBar;

        }

        public static Action Clear { get; set; }

        public static Action<string, float> Display { get; set; }
    }
}