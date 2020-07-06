using System.Collections.Generic;
using Elarion.Attributes;
using Elarion.Editor.Extensions;
using Elarion.Editor.GenericInspector.Drawers.ReorderableList;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Editor.GenericInspector.Drawers {
    public class ReorderableListDrawer : GenericInspectorDrawer {
        
        private readonly Dictionary<string, ExtendedReorderableList> _reorderableLists = new Dictionary<string, ExtendedReorderableList>();
        
        public ReorderableListDrawer(GenericInspector inspector, Object target, SerializedObject serializedObject) :
            base(inspector, target, serializedObject) {
            
            var iterator = SerializedObject.GetIterator();
            
            while(iterator.NextVisible(true)) {
                if(!iterator.isArray || iterator.propertyType == SerializedPropertyType.String) {
                    continue;
                }
                
                var attribute = iterator.GetAttribute<ReorderableAttribute>();
                    
                if(attribute == null || _reorderableLists.ContainsKey(iterator.propertyPath)) {
                    continue;
                }
                    
                var icon = !string.IsNullOrEmpty(attribute.ElementIconPath) ? AssetDatabase.GetCachedIcon(attribute.ElementIconPath) : null;

                var displayType = attribute.SingleLine ? ExtendedReorderableList.ElementDisplayType.SingleLine : ExtendedReorderableList.ElementDisplayType.Auto;

                var reorderableList = new ExtendedReorderableList(iterator.Copy(), 
                    attribute.Add, attribute.Remove, attribute.Draggable, displayType, 
                    attribute.ElementNameProperty, attribute.ElementNameOverride, icon, attribute.SceneReferences) 
                {
                    Paginate = attribute.Paginate, 
                    PageSize = attribute.PageSize, 
                    sortable = attribute.Sortable
                };
                _reorderableLists.Add(iterator.propertyPath, reorderableList);
            }
        }

        public override bool CanDrawProperty(SerializedProperty property) {
            if(!property.isArray || property.propertyType == SerializedPropertyType.String || !_reorderableLists.ContainsKey(property.propertyPath)) {
                return false;
            }

            return true;
        }

        public override void DrawProperty(SerializedProperty property) {
            _reorderableLists[property.propertyPath].DoLayoutList();
        }
    }
}