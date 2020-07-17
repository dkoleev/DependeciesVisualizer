using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes {
    public abstract class BaseNode : ScriptableObject {
        public Rect WindowRect;
        public string WindowTitle;

        public virtual void DrawWindow() {
            
        }

        public virtual void DrawCurve() {
            
        }
    }
}
