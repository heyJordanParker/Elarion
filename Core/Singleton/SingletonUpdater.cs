using UnityEngine;

namespace Elarion.Singleton {
    public class SingletonUpdater : SingletonNew<SingletonUpdater> {
        
        // TODO create a singleton updater (that calls the builtin methods for all singletons during runtime); use a proxy object (that just has events for everything without actual functionality)
        
        // TODO add method stubs to the base class & call them in the updater
        
        // TODO create a SingletonInspector - a window showing all singletons and their current state; show warnings if one of the singletons doesn't have the [Serializable] attribute
        
        // TODO create a protected event OnSingletonCreated/Destroyed and use it to fascilitate hooking the singletons with their update functions
        
        // TODO create a base inspector for singletons that inlines the inspector for the scriptableobject inside a property drawer. Example here: https://answers.unity.com/questions/1034777/draw-scrptableobject-inspector-in-other-inspector.html
        
        // TODO get the singletons to create/assign inside the editor
        
        // TODO add unit tests
        
        // TODO open source
        
        // TODO release as a free asset on the asset store

        public float test = 2;

//        [RuntimeInitializeOnLoadMethod]
        public static void Initialize() {
            // Create a hidden proxy object to call the builtin functions of all singletons
        }
        
    }
}