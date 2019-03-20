using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elarion.Editor.Extensions {
    public static class ReflectionExtensions {
        
        public static T GetFieldValue<T>(this object obj, string name) {
            var field = obj.GetType()
                .GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (T) field?.GetValue(obj);
        }

        public static IEnumerable<MethodInfo> GetMethodsRecursive(this Type type) {
            if(type == null)
                return Enumerable.Empty<MethodInfo>();

            const BindingFlags methodBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return type.GetMethods(methodBindingFlags).Concat(GetMethodsRecursive(type.BaseType));
        }
        
        public static IEnumerable<PropertyInfo> GetPropertiesRecursive(this Type type) {
            if(type == null)
                return Enumerable.Empty<PropertyInfo>();

            const BindingFlags propertyBindingFlags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return type.GetProperties(propertyBindingFlags).Concat(GetPropertiesRecursive(type.BaseType));
        }
        
    }
}