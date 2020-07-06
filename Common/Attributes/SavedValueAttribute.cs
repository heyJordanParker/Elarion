using System;

namespace Elarion.Attributes {
    /// <summary>
    /// Internal attribute used instead of GetGenericTypeDefinition() which throws an exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SavedValueAttribute : Attribute { }
}