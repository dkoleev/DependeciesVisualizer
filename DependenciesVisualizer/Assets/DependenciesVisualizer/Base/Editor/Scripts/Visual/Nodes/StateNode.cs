using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes {
    public class StateNode : BaseNode {
        public StateNode(Assembly assembly, Vector2 position, Vector2 size, string title) : base(assembly, position, size, title) { }

        public override void DrawWindow() {
            base.DrawWindow();
        }

        public override void DrawCurve() {
            base.DrawCurve();
        }
    }
}
