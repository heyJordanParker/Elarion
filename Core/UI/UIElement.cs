namespace Elarion.UI {
    public class UIElement : UIPanel {
        // TODO use this class for reusable UIElements - Sidebars, menus, and such 
        
        // Appear AFTER the transition or appear WITH the screen transition (the latter option matches the appearing type with the screen transition type and starts alongside the transition); hook this up via events in the UIManager
        
        // appear, disappear animations; follow the UIScreen structure as much as possible (it needs to be somewhat intuitive)
        
        // focus (when it comes to the background behind a popup for example), click, hover, select, loading animations (outlines, scale, overlays, blurs, etc)
        
        // UIElements that show ONLY during a transition - loaders and such
        
        // List of UIScreens for the element to be active in (
        
        // Hide during transitions option (animated stuff, performance?)
        
        // option to appear on screen level (below any transition effects)
        
        // appear over effects - blur/overlay etc (loaders and such)
        
        // register UI element in the UIManager
        
        // global elements e.g. popups that render over everything else; maybe render priority or something
        
        // layer for rendering - to help manage popups and similar stuff
        
        // UI Elements (e.g. top & bottom bars, popups) - visibility options - for native & mobile (screen orientation/size)
        
        // Optional Apply an effect over everything else (on a layer below the current one) - blur and/or color overlay (useful for popups & menus)
        
        // Deactivate method (with a callback) that allows the element to animate out of view; just disables the element if no type is set up
    }
}