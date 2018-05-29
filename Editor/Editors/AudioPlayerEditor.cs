using Elarion.Audio;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(AudioPlayer), true, isFallback = true)]
    public class AudioPlayerEditor : UnityEditor.Editor {
        private AudioPlayer Target {
            get { return target as AudioPlayer; }
        }

        private AudioSource _helper;

        private void OnEnable() {
            if(EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }

            _helper = new GameObject("Helper").AddComponent<AudioSource>();
            _helper.gameObject.AddComponent<AudioListener>();
            _helper.gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        }

        private void OnDisable() {
            if(EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }

            DestroyHelper();
        }

        private void DestroyHelper() {
            if(_helper != null) {
                DestroyImmediate(_helper.gameObject);            
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

            if(GUILayout.Button("Play"))
                Target.Play(_helper);

            GUI.enabled = true;
            
            if(EditorApplication.isPlayingOrWillChangePlaymode) {
                DestroyHelper();
            }
        }
    }
}