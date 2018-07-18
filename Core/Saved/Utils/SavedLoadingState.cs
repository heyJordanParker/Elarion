using Elarion.Saved.Variables.References;
using UnityEngine;

namespace Elarion.Saved.Utils {
    // loading service?
    public class SavedLoadingState {
        [SerializeField]
        private BoolReference _isLoading;

        public bool IsLoading => _isLoading.Value;

        // TODO a list of that objects add/remove themselves to when they're loading
        // TODO a IsLoading variable that returns true if the list isn't empty

        // minor - when objects register - they can specify a loading duration - loaders can use that to display progress bars (or endless bars if the duration isn't available - or is available for just *some* of the items) 
        // optional loading timeout

        // TODO loading bar/screen helper - shows/hides UIPanels based on loading state
    }
}