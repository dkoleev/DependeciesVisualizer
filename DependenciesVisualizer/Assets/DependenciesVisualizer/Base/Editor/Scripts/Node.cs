using System.Collections.Generic;
using System.Linq;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class Node {
        public Rect WindowRect;

        public Assembly Assembly { get; }
        private IList<Node> _inputDependencies;
        private IList<Node> _outputDependencies;
        private static Vector2 _defaultSize = new Vector2(300, 100);
        private readonly string _windowTitle;
        private NodeVisual _mainVisual;
        private NodeVisual _cycleDependentVisual;
        private LayersWindow _layersWindow;
        private VisualizerPreferences _preferences;
        private NodeData _data;
        private static Texture2D _backTexture;

        public Node(Assembly assembly, LayersWindow layersWindow, NodeData data) {
            Assembly = assembly;
            _layersWindow = layersWindow;
            _data = data;
            WindowRect = new Rect(_data.Position, _defaultSize);
            _windowTitle = Assembly.name;

            _mainVisual = new NodeVisual {
                LineColor = new Color32(90, 145, 60, 255),
                LineShadowColor = new Color32(50,55,35,255)
            };
            
            _cycleDependentVisual = new NodeVisual {
                LineColor = new Color32(150, 45, 45, 255),
                LineShadowColor = new Color32(70,15,15,255)
            };

            InitBackground();
        }

        private void InitBackground() {
            _backTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _backTexture.SetPixel(0, 0, new Color(0.25f, 0.4f, 0.25f));
            _backTexture.Apply();
        }

        public void SetPosition(Vector2 position) {
            _data.Position = position;
            WindowRect.position = position;
        }

        public int GetDependencyLevel(int startLevel) {
            if (HaveInputDependencies()) {
                startLevel++;
                var bufLevel = startLevel;
                foreach (var dependency in _inputDependencies) {
                    var depLevel = dependency.GetDependencyLevel(startLevel);
                    if (depLevel > bufLevel) {
                        bufLevel = depLevel;
                    }
                }

                startLevel = bufLevel;
            }

            return startLevel;
        }

        public void InjectInputReferences(IList<Node> references) {
            _inputDependencies = references;
        }
        
        public void InjectOutputReferences(IList<Node> references) {
            _outputDependencies = references;
        }

        public bool HaveInputDependencies() {
            return _inputDependencies.Count > 0;
        }
        
        public bool HaveOutputDependencies() {
            return _outputDependencies.Count > 0;
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
            GUI.backgroundColor = _layersWindow.Layers[_data.CurrentLayer].Color;
            WindowRect = GUI.Window(
                id, 
                WindowRect,
                i => {
                    DrawLayersPopup();
                    GUI.DragWindow();
                }, 
                _windowTitle);
            
            _data.Position = WindowRect.position;
            GUI.backgroundColor = Color.white;
        }
        

        private void DrawLayersPopup() {
            var newLayer = -1;

            var layers = _layersWindow.Layers.ToArray();
            var names = new List<string>();
            foreach (var layer in  layers) {
                names.Add(layer.Name);

                if (newLayer == -1 && layer.Name == _data.CurrentLayerName) {
                    _data.CurrentLayer = layer.Priority;
                    newLayer = _data.CurrentLayer;
                }
            }


            _data.CurrentLayer = EditorGUILayout.Popup("Layer: ", _data.CurrentLayer, names.ToArray());

            if (newLayer != _data.CurrentLayer) {
                _data.CurrentLayerName = _layersWindow.Layers[_data.CurrentLayer].Name;
            }
        }

        public void DrawOutputReferences() {
            foreach (var reference in _outputDependencies) {
                var isCycleDependent = IsInput(reference);
                DrawCurveReferences(WindowRect, reference.WindowRect, isCycleDependent? _cycleDependentVisual : _mainVisual);
            }
        }

        private void DrawCurveReferences(Rect start, Rect end, NodeVisual visual) {
            var startPos = new Vector3(
                start.x + start.width * 0.5f,
                start.y + start.height,
                0
            );
            var endPos = new Vector3(
                end.x + end.width * 0.5f,
                end.y,
                0
            );

            var startTan = startPos + Vector3.up * 50;
            var endTan = endPos + Vector3.down * 50;

            
            for (var i = 1; i < 4; i++) {
                var shadow = new Color32(visual.LineShadowColor.r,visual.LineShadowColor.g,visual.LineShadowColor.b,(byte)Mathf.Clamp(i*30f, 0, 255));
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, i*2);
            }
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, visual.LineColor, null, 3);
        }

        public void Remove() {
            
        }

        public void Restore() {
            
        }

        private struct NodeVisual {
            public Color32 LineColor;
            public Color32 LineShadowColor;
        }
    }
}