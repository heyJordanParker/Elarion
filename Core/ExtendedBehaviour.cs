using System.Diagnostics;
using System.Reflection;
using Elarion.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Elarion {
    public abstract class ExtendedBehaviour : MonoBehaviour {
        
        /// <summary>
        ///   <para>Returns true if the GameObject and the Component are active.</para>
        /// </summary>
        /// <returns>
        ///   <para>Active.</para>
        /// </returns>
        public virtual bool IsActive() {
            return isActiveAndEnabled;
        }

        /// <summary>
        ///   <para>Returns true if the native representation of the behaviour has been destroyed.</para>
        /// </summary>
        /// <returns>
        ///   <para>True if Destroyed.</para>
        /// </returns>
        public bool IsDestroyed() {
            return this == null;
        }

        protected virtual void Reset() {
            WriteDefaults();
        }

        protected virtual void OnValidate() {
            WriteDefaults();
        }

        [Conditional("UNITY_EDITOR")]
        protected virtual void OnWriteDefaults() { }

        [Conditional("UNITY_EDITOR")]
        private void WriteDefaults() {
            OnWriteDefaults();

            #if UNITY_EDITOR
            if(UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this)) {
                return;
            }
            #endif
            
            var type = GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for(int i = 0; i < fields.Length; ++i) {
                var field = fields[i];

                if(field.GetCustomAttributes(typeof(GetComponentAttribute), false).Length < 1) {
                    continue;
                }

                if(!field.IsPublic && field.GetCustomAttributes(typeof(SerializeField), false).Length < 1) {
                    Debug.LogError($"Trying to automatically GetComponent on a non-serialized field {field.Name}",
                        this);
                    continue;
                }

                if(!field.FieldType.IsSubclassOf(typeof(Component))) {
                    Debug.LogError(
                        $"Trying to automatically GetComponent for the {field.Name} field, but the field doesn't inherit from UnityEngine.Component.",
                        this);
                    continue;
                }

                var value = field.GetValue(this);

                if(value != null) {
                    var stringValue = value.ToString();

                    if(stringValue.Length > 0 && stringValue.ToLowerInvariant() != "null") {
                        continue;
                    }
                }

                var component = GetComponent(field.FieldType);

                if(component == null) {
                    Debug.LogError(
                        $"Trying to automatically GetComponent for the {field.Name} field, but a component of that type is missing. You can use [RequireComponent] to ensure that the component will be available.",
                        gameObject);
                    continue;
                }

                field.SetValue(this, component);
            }
        }
    }
}