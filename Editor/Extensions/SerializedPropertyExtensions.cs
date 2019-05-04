using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Elarion.Editor.Extensions {
    public static class SerializedPropertyExtensions {
        public const BindingFlags SerializedPropertyBindingFlags = BindingFlags.GetField
                                                                    | BindingFlags.GetProperty
                                                                    | BindingFlags.Instance
                                                                    | BindingFlags.NonPublic
                                                                    | BindingFlags.Public;

        private const string ArrayPathSignature = ".Array.data[";

        public static T GetBaseProperty<T>(this SerializedProperty prop) {
            // Separate the steps it takes to get to this property
            var path = prop.propertyPath.Replace(ArrayPathSignature, "[");
            object targetObject = prop.serializedObject.targetObject;
            var separatedPaths = path.Split('.');

            // Go down to the root of this serialized property
            foreach(var element in separatedPaths) {
                if(element.Contains("[")) {
                    // Array
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    targetObject = GetArrayFieldValue(targetObject, elementName, index);
                } else {
                    // Object
                    targetObject = GetFieldValue(targetObject, element);
                }
            }

            return (T) targetObject;
        }
        
        public static string GetBaseObjectPath(this SerializedProperty property) {
            var parent = property.propertyPath;
            var firstDot = property.propertyPath.IndexOf('.');
            if(firstDot > 0) {
                parent = property.propertyPath.Substring(0, firstDot);
            }

            return parent;
        }

        public static object GetObject(this SerializedProperty prop) {
            var path = prop.propertyPath.Replace(ArrayPathSignature, "[");
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

        public static Type GetFieldType(this SerializedProperty property) {
            if(property == null) {
                return null;
            }
            
            var type = property?.GetObject()?.GetType();
            
            return GetFieldTypeRecursive(type, property.name);
        }

        private static Type GetFieldTypeRecursive(Type type, string fieldName) {
            while(true) {
                if(type == null) {
                    return null;
                }

                var field = type.GetField(fieldName, SerializedPropertyBindingFlags);

                if(field != null) {
                    return field.FieldType;
                }
                
                type = type.BaseType;
            }
        }

        public static object[] GetAttributes<T>(this SerializedProperty property) where T : Attribute {
            var attributeType = typeof(T);

            var field = property?.GetObject()?.GetType().GetField(property.name, SerializedPropertyBindingFlags);

            return field != null ? field.GetCustomAttributes(attributeType, true) : null;
        }

        public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute {
            var attributes = GetAttributes<T>(property);
            if(attributes != null && attributes.Length > 0) {
                return attributes[0] as T;
            }

            return null;
        }

        public static bool HasAttribute<T>(this SerializedProperty property) where T : Attribute {
            var attributes = GetAttributes<T>(property);
            return attributes != null && attributes.Length > 0;
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

        public static bool IsInArray(this SerializedProperty property) {
            return property.ArrayIndex() != -1;
        }

        public static int ArrayIndex(this SerializedProperty property) {
            if(property == null) {
                return -1;
            }
            var path = property.propertyPath;

            var startIndex = path.LastIndexOf(ArrayPathSignature, StringComparison.Ordinal);
            var endIndex = path.LastIndexOf("]", StringComparison.Ordinal);
            
            if(startIndex == -1 || endIndex == -1 || startIndex >= endIndex) {
                return -1;
            }

            int.TryParse(path.Substring(startIndex + ArrayPathSignature.Length).Replace("]", ""), out var result);
            
            return result;
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