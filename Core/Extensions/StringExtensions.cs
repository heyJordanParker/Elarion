using System;

namespace Elarion.Extensions {
    public static class StringExtensions {
        public static T ToEnum<T>(this string value) {
            //Null check
            if(value == null) throw new ArgumentNullException("value");
            //Empty string check
            value = value.Trim();
            if(value.Length == 0) throw new ArgumentException("Must specify valid information for parsing in the string", "value");
            //Not enum check
            Type t = typeof(T);
            if(!t.IsEnum) throw new ArgumentException("Type provided must be an Enum", "T");

            return (T)Enum.Parse(typeof(T), value);
        }

        public static string SFormat(this string value, params object[] parameters) {
            return string.Format(value, parameters);
        }
    }
}