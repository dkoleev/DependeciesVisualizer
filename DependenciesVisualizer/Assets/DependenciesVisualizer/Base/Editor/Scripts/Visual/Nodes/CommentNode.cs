using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes {
    public class CommentNode : BaseNode {
        private string _comment = "New comment";
        public CommentNode(Assembly assembly, Vector2 position, Vector2 size, string title) 
            : base(assembly, position, size, title) {
            _comment = GUILayout.TextArea(_comment, 200);
        }
    }
}
