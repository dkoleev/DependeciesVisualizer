using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Base {
    public class Node {
        public Rect WindowRect { get; private set; }

        public Assembly Assembly { get; }
        private IList<Node> _references;
        private static Vector2 _defaultSize = new Vector2(250, 100);
        private readonly string _windowTitle;

        public Node(Assembly assembly, Vector2 position) {
            Assembly = assembly;
            WindowRect = new Rect(position, _defaultSize);
            _windowTitle = Assembly.name;
        }

        public void InjectReferences(IList<Node> references) {
            _references = references;
        }
        
        public bool IsDependent(Node node) {
            return Assembly.assemblyReferences.Contains(node.Assembly);
        }

        public void Draw(int id) {
            WindowRect = GUI.Window(
                id, 
                WindowRect, 
                i => { GUI.DragWindow(); }, 
                _windowTitle);
        }

        public void DrawInputReferences() {
            foreach (var reference in _references) {
                DrawCurveReferences(WindowRect, reference.WindowRect, true, new Color32(100, 130,80, 255));
            }
        }

        private void DrawCurveReferences(Rect start, Rect end, bool left, Color curveColor) {
            var startPos = new Vector3(
                left ? start.x + start.width : start.x,
                start.y + start.height * 0.5f,
                0
            );
            var endPos = new Vector3(
                end.x + end.width * 0.5f,
                end.y + end.height * 0.5f,
                0
            );

            var startTan = startPos + Vector3.right * 50;
            var endTan = endPos + Vector3.left * 50;
            
            for (var i = 1; i < 4; i++) {
                var shadow = new Color32(50,55,35,(byte)Mathf.Clamp(i*30f, 0, 255));
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, i*2);
            }
            
            //var shadow = new Color(0,0,0,0.3f);
            //Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, 5);
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 3);
        }
    }
}