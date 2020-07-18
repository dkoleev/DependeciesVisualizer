using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes {
    public abstract class BaseNode : ScriptableObject {
        public static Vector2 DefaultSize = new Vector2(250, 100);
        public Rect WindowRect;
        public string WindowTitle;

        protected Assembly Assembly { get; }

        protected BaseNode(Assembly assembly, Vector2 position, Vector2 size, string title) {
            Assembly = assembly;
            
            WindowRect = new Rect(position, size);
            WindowTitle = title;
        }

        public void Remove() {
            
        }

        public void Restore() {
            
        }

        public virtual void DrawWindow() {
            
        }

        public virtual void DrawCurve() {
            
        }
    }
}
