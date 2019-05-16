using System;

namespace Elarion.Common.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : Attribute {

        public string Title { get; set;  }
        
        public string Label { get; }
        
        public bool AlwaysVisible { get; }
        
        public bool DrawInPlayMode { get; }
        
        public bool DrawInEditorMode { get; }

        public InspectorButtonAttribute(string title = null, string label = null, bool alwaysVisible = false, bool drawInPlayMode = true, bool drawInEditorMode = true) {
            Title = title;
            Label = label;
            AlwaysVisible = alwaysVisible;
            DrawInPlayMode = drawInPlayMode;
            DrawInEditorMode = drawInEditorMode;
        }
    }
}