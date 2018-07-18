using Elarion.UI.Helpers.Animation;
using UnityEngine;

namespace Elarion.UI.Utils {
    [RequireComponent(typeof(UIAnimator))]
    public class TouchScreenKeyboardAnimation : MonoBehaviour {

        public UIAnimation onKeyboardOpen;
        public UIAnimation onKeyboardClose;

        private UIAnimator _animator;
        private bool _keyboardVisible;

        private bool KeyboardVisible {
            get { return _keyboardVisible; }
            set {
                if(_keyboardVisible == value) {
                    return;
                }

                _keyboardVisible = value;
                
                _animator.Play(_keyboardVisible ? onKeyboardOpen : onKeyboardClose);
            }
        }

        private void Awake() {
            _animator = GetComponent<UIAnimator>();
        }

        public void Update() {
            KeyboardVisible = TouchScreenKeyboard.visible;
        }

    }
}