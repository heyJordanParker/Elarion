using System;

namespace Elarion.Common.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class RequiresConstantRepaintAttribute : Attribute { }
}