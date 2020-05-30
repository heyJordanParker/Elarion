using System;

namespace Elarion.Extensions {
    public static class StringExtensions {
        public static T ToEnum<T>(this string value) {
            //Null check
            if(value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            
            //Empty string check
            value = value.Trim();
            if(value.Length == 0) {
                throw new ArgumentException("Must specify valid information for parsing in the string", nameof(value));
            }
            
            //Not enum check
            var t = typeof(T);
            if(!t.IsEnum) {
                throw new ArgumentException("Type provided must be an Enum", "T");
            }

            return (T) Enum.Parse(typeof(T), value);
        }

        public static bool IsNullOrEmpty(this string value) {
            return string.IsNullOrEmpty(value);
        }

        public static string Format(this string value, params object[] args) {
            return string.Format(value, args);
        }
    }
}