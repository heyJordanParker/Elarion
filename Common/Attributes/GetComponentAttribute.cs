using System;
using UnityEngine;

namespace Elarion.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class GetComponentAttribute : PropertyAttribute { }
}