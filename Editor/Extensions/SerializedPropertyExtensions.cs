using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Extensions {
    public static class SerializedPropertyExtensions {
        public static T GetBaseProperty<T>(this SerializedProperty prop) {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            object reflectionTarget = prop.serializedObject.targetObject;
            // Walk down the path to get the target object
            foreach(var path in separatedPaths) {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }

            return (T) reflectionTarget;
        }

        public static Type GetBasePropertyType(this SerializedProperty prop) {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            object reflectionTarget = prop.serializedObject.targetObject;
            // Walk down the path to get the target object
            foreach(var path in separatedPaths) {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }

            return reflectionTarget.GetType();
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