namespace Elarion.Editor.Editors {
    public partial class UIComponentEditor {

//        private UIScene TargetScene {
//            get { return target as UIScene; }
//        }
//
//        private SerializedProperty initialSceneProperty;
//        
//        private void OnEnable() {
//            initialSceneProperty = serializedObject.FindProperty("_initialScene");
//        }
//        
//        private void DrawSceneInspectorGUI() {
//            this.DrawDefaultScriptField();
//
//            EditorGUILayout.PropertyField(initialSceneProperty);
//
//            if(!TargetScene.InitialScene) {
//                GUI.enabled = false;
//                EditorGUILayout.ObjectField("Initial Scene", UIScene.Scenes.FirstOrDefault(s => s.InitialScene), typeof(GameObject), true);
//                GUI.enabled = true;
//            }
//            
//            DrawHelpersGUI();
//            
//            serializedObject.ApplyModifiedProperties();
//        }
    }
}