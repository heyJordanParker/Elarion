using System;

namespace Elarion.UI {
    
    public class UIForm : UIDialog {

        [Serializable]
        public class UIFormInput {
            
            // type - toggle, slider, dropdown, input field (or a custom interface inheritor); TMP option
            // target - object reference
            // name - unique (for the input) string name (auto generate one)
            // value - property - returns the correct value based on the input type
            // required - bool
            // validation (if required and type is input field) - preset types & custom regex option
            // validation error (if required and validation is set) 
            // missing error (if required) - string
            
        }
        
        // TODO get all Selectable children and add them to the UIFormInput array
        
        
        // TODO loading state (for submitting/receiving requests and such)
        // TODO submit -> send a request to server & enter loading -> wait for response & exit loading -> either finish submission and close the form on OK or show error/errors  
        
        // TODO show errors on incorrect required fields on submit;
        // TODO focus empty required fields on submit (without showing errors)
    }
}