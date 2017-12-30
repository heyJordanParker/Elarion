using Elarion.Attributes;
using UnityEngine;

namespace Elarion.UI {
    
    // TODO resizable edge menu - also has min-max size + a resize method
    public class UIEdgeMenu : UIPanel {
        // TODO edge menu that imitates drag to update
        
        [Tooltip("Which edge of the screen should the menu appear from.")]
        public RectTransform.Edge edge;

        [Tooltip("Percent of screen to cover."), ConditionalVisibility("absoluteSize")]
        public float size = 0.3f;
        
        [ConditionalVisibility("!absoluteSize, edge == RectTransform.Edge.Left || RectTransform.Edge.Right")]
        public float width = 300;
        
        [ConditionalVisibility("!absoluteSize, edge == RectTransform.Edge.Top || RectTransform.Edge.Bottom")]
        public float height = 300;

        [Tooltip("Set to true if you want to size your edge in pixels and not percent.")]
        public bool absoluteSize = false;

        protected override int Width {
            get {
                if(edge != RectTransform.Edge.Left && edge != RectTransform.Edge.Right) {
                    return base.Width;
                }
                
                if(absoluteSize) {
                    return Mathf.FloorToInt(width);
                }
                
                return Mathf.FloorToInt(base.Width * size);
            }
        }

        protected override int Height {
            get {
                if(edge != RectTransform.Edge.Top && edge != RectTransform.Edge.Bottom) {
                    return base.Height;
                }

                if(absoluteSize) {
                    return Mathf.FloorToInt(height);
                }
                
                return Mathf.FloorToInt(base.Height * size);
            }
        }

        public override void Show() {
            base.Show();
            InTransition = true;
            
            
            // UIManager -> current screen should blur
            // reuse the transition code from the UIScreen
            // set the anchors of the EdgeMenu so that its' fully visible at the (0,0) position
            // if screensize changes -> update size (if it's not fixed)
        }

        public override void Hide() {
            base.Hide();
            InTransition = true;
        }
    }
}