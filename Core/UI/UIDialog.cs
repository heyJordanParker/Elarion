using UnityEngine;
using UnityEngine.UI;

namespace Elarion.UI {
    
    // TODO Popup prefabs - create a default one (and auto load it when instantiating via EditorMenus) and leave it as a public field so users can change it with their own

    // TODO input validator component - a few builtin options and a custom regex option (as enum) and a length validator (with minmax slider)
    
    // TODO use the OnSubmit/OnCancel handlers to add cancelation/submitting to regular unity input fields and such - just add them on runtime and hook the appropriate events

    public class UIPopup : UIDialog {
        // basic info dialog - both submit and cancel close it; minimal inputs
    }
    
    public class UIForm : UIDialog {
        // Add those below on the first update
        // input field class - key and value (both strings for easy www calls), optional validation, etc
        // auto-focus invalid input fields
        // show errors
        
        // TODO UIForm inheritor - add error checking submitting and so on builtin (submit with enter/submit input (in unity))
        
        // TODO get all inputfields and button children on awake
        // TODO add onsubmit event to all inputfields and add a submit event to the button
    }
    
    [RequireComponent(typeof(UISubmitHandler))]
    [RequireComponent(typeof(UICancelHandler))]
    public class UIDialog : UIPanel {
        
        // TODO cache the object that was last focused before opening this and focus it back when closing
        
        // dialog skins?

        // dropdown DeselectAction
        public bool submitOnDeselect;
        public bool cancelOnDeselect;

        private InputField[] _inputs;

        protected override void Awake() {
            base.Awake();

            _inputs = GetComponentsInChildren<InputField>();
        }

        public override void Focus(bool setSelection = false) {
            // always focus the first dialog child if clicked
            base.Focus(true);
        }

        // TODO add submit, cancel hanlders here

    }
}