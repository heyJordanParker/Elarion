using System;
using System.Reflection;
using UnityEditor;

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

            return (T)reflectionTarget;
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
    }
}