using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elarion.Common;
using Elarion.Editor.Extensions;
using Elarion.Editor.Plugins.AutocompleteSearchField;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    /// <summary>
    /// Serializable type drawer. This class assumes that you're using the GenericInspector and that you've neatly placed all your classes in the respective assemblies.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeAttributeDrawer : PropertyDrawer {
        private class Drawer {
            private readonly AutocompleteSearchField _assemblySearch;

            private List<Assembly> _selectedAssemblies;
            private Type[] _selectedTypes;

            private readonly SerializedProperty _targetProperty;
            private Type _selectedType;

            public Drawer(SerializedProperty property) {
                _targetProperty = property.FindPropertyRelative("_assemblyQualifiedName");
                _selectedType = Type.GetType(_targetProperty.stringValue);

                _assemblySearch = new AutocompleteSearchField();
                _assemblySearch.searchString = _selectedType != null ? _selectedType.FullName : "";
                _assemblySearch.inPropertyDrawer = true;
                _assemblySearch.maxResults = 25;
                _assemblySearch.onInputChanged += OnInputChanged;
                _assemblySearch.onConfirm += OnConfirm;
            }
            
            public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                if(_selectedType != Type.GetType(_targetProperty.stringValue)) {
                    _targetProperty.stringValue = _selectedType?.AssemblyQualifiedName;
                    property.serializedObject.ApplyModifiedProperties();
                }

                var typeName = _selectedType != null ? _selectedType.Name : "None"; 

                label.text += $" ({typeName})";

                EditorGUI.BeginProperty(position, label, property);

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), label);

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                _assemblySearch.Draw(position);

                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }

            void OnInputChanged(string searchString) {
                _assemblySearch.ClearResults();

                if(string.IsNullOrEmpty(searchString)) {
                    _selectedAssemblies = null;
                    _selectedTypes = null;
                    return;
                }

                if(_assemblyCache == null) {
                    _assemblyCache = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray();
                }

                var results = new List<string>();

                _selectedAssemblies = _assemblyCache.Where(a =>
                    a.GetName().Name.StartsWith(_assemblySearch.searchString,
                        StringComparison.InvariantCultureIgnoreCase) ||
                    _assemblySearch.searchString.StartsWith(a.GetName().Name,
                        StringComparison.InvariantCultureIgnoreCase)).ToList();

                if(_selectedAssemblies.Count > 1) {
                    results.AddRange(_selectedAssemblies.Select(assembly => assembly.GetName().Name));
                }

                if(searchString.StartsWith("System", StringComparison.InvariantCultureIgnoreCase)) {
                    // add mscorlib (the System namespace is there)
                    _selectedAssemblies.Add(typeof(int).Assembly);
                }

                if(searchString.Length > 3) {
                    _selectedTypes = _selectedAssemblies.SelectMany(a => a.GetExportedTypes()).Where(t =>
                        t.FullName != null &&
                        t.FullName.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase)).ToArray();

                    results.AddRange(_selectedTypes.Select(type => type.FullName));
                }

                results.Sort();

                foreach(var searchResult in results.Distinct()) {
                    _assemblySearch.AddResult(searchResult);
                }
            }

            void OnConfirm(string result) {
                _selectedType = _selectedTypes?.FirstOrDefault(t => t.FullName == result);
                _assemblySearch.searchString = _selectedType?.FullName ?? "";
                _assemblySearch.ClearResults();
            }
        }

        private static Assembly[] _assemblyCache;

        // this approach supports arrays of those; Unity shares an instance of the Drawer for every array (which we can't work with since we need separate search fields)
        private readonly Dictionary<string, Drawer> _drawers = new Dictionary<string, Drawer>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var path = property.propertyPath;
            if(!_drawers.ContainsKey(path)) {
                _drawers.Add(path, new Drawer(property));
            }
            
            _drawers[path].OnGUI(position, property, label);
        }
    }
}