using System;

namespace Elarion.Editor.Extensions {
    public static class TypeExtensions {
        public static bool IsSubclassOfRawGeneric(this Type type, Type genericType) {
            while (type != null && type != typeof(object)) {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericType == cur) {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}