namespace Elarion.UI {
    
    public class UIPopup : UIDialog {
        // basic info dialog - both submit and cancel close it; minimal inputs
    }
    
    public class UIForm : UIDialog {
        // Add those below on the first update
        // input field class - key and value (both strings for easy www calls), optional validation, etc
        // auto-focus invalid input fields
        // show errors
        
        // TODO show errors on incorrect required fields on submit; just focus empty required fields on submit otherwise (without showing the errors)
        
        // TODO UIForm inheritor - add error checking submitting and so on builtin (submit with enter/submit input (in unity))
        
        // TODO get all inputfields and button children on awake
        // TODO add onsubmit event to all inputfields and add a submit event to the button
    }
}