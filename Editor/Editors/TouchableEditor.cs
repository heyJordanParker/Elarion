using Elarion.UI;
using UnityEditor;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(Touchable))]
    [CanEditMultipleObjects]
    public class TouchableEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() { }
    }
}
