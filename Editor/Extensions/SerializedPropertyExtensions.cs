using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Extensions {
    public static class SerializedPropertyExtensions {
        public static T GetBaseProperty<T>(this SerializedProperty prop) {
            // Separate the steps it takes to get to this property
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object targetObject = prop.serializedObject.targetObject;
            var separatedPaths = path.Split('.');
            
            // Go down to the root of this serialized property
            foreach(var element in separatedPaths) {
                
                if(element.Contains("[")) {
                    // Array
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    targetObject = GetArrayFieldValue(targetObject, elementName, index);
                } else {
                    // Object
                    targetObject = GetFieldValue(targetObject, element);
                }
            }

            return (T) targetObject;
        }

        public static object GetTargetObject(this SerializedProperty prop) {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object targetObject = prop.serializedObject.targetObject;
            var separatedPaths = path.Split('.');
            
            foreach(var element in separatedPaths.Take(separatedPaths.Length - 1)) {
                
                if(element.Contains("[")) {
                    // Array
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    targetObject = GetArrayFieldValue(targetObject, elementName, index);
                } else {
                    // Object
                    targetObject = GetFieldValue(targetObject, element);
                }
            }
            
            return targetObject;
        }
        
        public static object GetValue(this SerializedProperty property) {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.GetValue(property.serializedObject.targetObject);
        }

        public static void SetValue(this SerializedProperty property, object value) {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetFieldViaPath(property.propertyPath);
            fi.SetValue(property.serializedObject.targetObject, value);
        }

        public static Type GetUnderlyingType(this SerializedProperty property) {
            var parentType = property.serializedObject.targetObject.GetType();
            var fi = parentType.GetFieldViaPath(property.propertyPath);
            return fi.FieldType;
        }

        public static FieldInfo GetFieldViaPath(this Type type, string path) {
            var containingObjectType = type;
            var fi = type.GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            if(!path.Contains('.')) {
                return fi;
            }
            
            var perDot = path.Split('.');
            foreach(var fieldName in perDot) {
                fi = containingObjectType.GetField(fieldName);
                if(fi != null) {
                    containingObjectType = fi.FieldType;
                } else {
                    return null;
                }
            }

            return fi;
        }

        private static object GetFieldValue(object source, string name) {
            if(source == null)
                return null;
            var type = source.GetType();

            while(type != null) {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if(f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetArrayFieldValue(object source, string name, int index) {
            var enumerable = GetFieldValue(source, name) as System.Collections.IEnumerable;
            
            if(enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for(int i = 0; i <= index; i++) {
                if(!enm.MoveNext()) return null;
            }
            
            return enm.Current;
        }
    }
}