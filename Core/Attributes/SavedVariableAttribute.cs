using System;

namespace Elarion.Attributes {
    /// <summary>
    /// Internal attributed used instead of GetGenericTypeDefinition() which throws an exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SavedVariableAttribute : Attribute { }
}