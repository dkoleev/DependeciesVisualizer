using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Base {
    public class Node {
        public Rect WindowRect { get; private set; }

        public Assembly Assembly { get; }
        private IList<Node> _inputDependencies;
        private IList<Node> _outputDependencies;
        private static Vector2 _defaultSize = new Vector2(250, 100);
        private readonly string _windowTitle;
        private NodeVisual _mainVisual;
        private NodeVisual _cycleDependentVisual;

        public Node(Assembly assembly, Vector2 position) {
            Assembly = assembly;
            WindowRect = new Rect(position, _defaultSize);
            _windowTitle = Assembly.name;
            
            _mainVisual = new NodeVisual {
                LineColor = new Color32(90, 145, 60, 255),
                LineShadowColor = new Color32(50,55,35,255)
            };
            
            _cycleDependentVisual = new NodeVisual {
                LineColor = new Color32(150, 45, 45, 255),
                LineShadowColor = new Color32(70,15,15,255)
            };
        }

        public void InjectInputReferences(IList<Node> references) {
            _inputDependencies = references;
        }
        
        public void InjectOutputReferences(IList<Node> references) {
            _outputDependencies = references;
        }
        
        public bool IsInput(Node node) {
            return _inputDependencies.Contains(node);
        }
        
        public bool IsOutput(Node node) {
            return _outputDependencies.Contains(node);
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
            foreach (var reference in _inputDependencies) {
                var isCycleDependent = IsOutput(reference);
                DrawCurveReferences(WindowRect, reference.WindowRect, true, isCycleDependent? _cycleDependentVisual : _mainVisual);
            }
        }

        private void DrawCurveReferences(Rect start, Rect end, bool left, NodeVisual visual) {
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
                var shadow = new Color32(visual.LineShadowColor.r,visual.LineShadowColor.g,visual.LineShadowColor.b,(byte)Mathf.Clamp(i*30f, 0, 255));
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, i*2);
            }
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, visual.LineColor, null, 3);
        }
        
        private struct NodeVisual {
            public Color32 LineColor;
            public Color32 LineShadowColor;
        }
    }
}