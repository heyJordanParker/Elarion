namespace Elarion.Editor.PropertyDrawers.Helpers {
    public class ConditionalLabel {
        public readonly string label;
        public readonly VisibilityCondition[] visibilityConditions;
        
        public ConditionalLabel(string label, VisibilityCondition[] visibilityConditions) {
            this.visibilityConditions = visibilityConditions;
            this.label = label;
        }
    }
}