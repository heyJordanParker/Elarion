using System;

namespace Elarion.Common.Attributes {
    /// <summary>
    /// Internal attribute used instead of GetGenericTypeDefinition() which throws an exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SavedListAttribute : Attribute { }
}