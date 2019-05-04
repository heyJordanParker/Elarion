using System;

namespace Elarion.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class RequiresConstantRepaintAttribute : Attribute { }
}