using System;
using System.Collections.Generic;

namespace Elarion.Extensions {
    public static class EnumExtensions {

        public static T AddFlag<T>(this Enum type, T enumFlag) {
            try {
                return (T)(object)((int)(object)type | (int)(object)enumFlag);
            } catch(Exception ex) {
                throw new ArgumentException(string.Format("Could not append flag value {0} to enum {1}", enumFlag, typeof(T).Name), ex);
            }
        }

        public static T RemoveFlag<T>(this Enum type, T enumFlag) {
            try {
                return (T)(object)((int)(object)type & ~(int)(object)enumFlag);
            } catch(Exception ex) {
                throw new ArgumentException(string.Format("Could not remove flag value {0} from enum {1}", enumFlag, typeof(T).Name), ex);
            }
        }

        public static bool HasFlag<T>(this Enum type, T enumFlag) {
            try {
                return ((int)(object) type & (int)(object) enumFlag) != 0;
            } catch(Exception ex) {
                throw new ArgumentException(string.Format("Could not get flag value {0} from enum {1}", enumFlag, typeof(T).Name), ex);
            }
        }

        public static T SetFlag<T>(this Enum type, T enumFlag, bool value) {
            return value ? type.AddFlag(enumFlag) : type.RemoveFlag(enumFlag);
        }

        /// <summary>
        /// Checks if the flag value is identical to the provided enum.
        /// </summary>
        public static bool IsIdenticalFlag<T>(this Enum type, T enumFlag) {
            try {
                return (int)(object)type == (int)(object)enumFlag;
            } catch {
                return false;
            }
        }

    }
}