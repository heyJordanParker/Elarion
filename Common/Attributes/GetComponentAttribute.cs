using System;
using UnityEngine;

namespace Elarion.Common.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class GetComponentAttribute : PropertyAttribute { }
}