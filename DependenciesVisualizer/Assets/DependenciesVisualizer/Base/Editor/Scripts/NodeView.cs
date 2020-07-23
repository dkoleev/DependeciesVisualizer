using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Models;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class NodeView {
        public Rect WindowRect;
        public Node Model => _model;

        private static Vector2 _defaultSize = new Vector2(300, 100);
        private readonly string _windowTitle;
        private NodeVisual _mainVisual;
        private NodeVisual _cycleDependentVisual;
        private LayersWindow _layersWindow;
        private VisualizerState _state;
        private Node _model;
        private NodeData _data => _model.Data;
        private MainWindow _mainWindow;

        public NodeView(MainWindow mainWindow, LayersWindow layersWindow, Node model) {
            _mainWindow = mainWindow;
            _layersWindow = layersWindow;
            _model = model;
            WindowRect = new Rect(model.Data.Position, _defaultSize);
            _windowTitle = _model.Assembly.name;

            _mainVisual = new NodeVisual {
                LineColor = new Color32(90, 145, 60, 255),
                LineShadowColor = new Color32(50,55,35,255)
            };
            
            _cycleDependentVisual = new NodeVisual {
                LineColor = new Color32(150, 45, 45, 255),
                LineShadowColor = new Color32(70,15,15,255)
            };
        }

        public void SetPosition(Vector2 position) {
            _data.Position = position;
            WindowRect.position = position;
        }

        public void Draw(int id) {
            GUI.backgroundColor = _layersWindow.Layers[_data.CurrentLayer].Color;
            WindowRect = GUI.Window(
                id, 
                WindowRect,
                i => {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    DrawLayersPopup();
                    DrawInputOutputAmount();
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

        private void DrawInputOutputAmount() {
            EditorGUI.LabelField(new Rect(10, WindowRect.height - 20, 100, 50), 
                $"Input: {_model.InputDependencies.Count} Output: {_model.OutputDependencies.Count}",
                new GUIStyle {
                    fontSize = 11
                });
        }

        public void DrawOutputReferences(Texture2D arrowTexture) {
            foreach (var reference in _model.OutputDependencies) {
                var isCycleDependent = _model.IsInput(reference);
                var view = _mainWindow.GetNodeViewByModel(reference);
                DrawCurveReferences(WindowRect, view.WindowRect, isCycleDependent? _cycleDependentVisual : _mainVisual, arrowTexture);
            }
        }

        private void DrawCurveReferences(Rect start, Rect end, NodeVisual visual, Texture2D arrowTexture) {
            var xOffset = start.position.x > end.position.x ? 10 : -10;

            var startPos = new Vector3(
                start.x + start.width * 0.5f,
                start.y + start.height,
                0
            );
            var endPos = new Vector3(
                end.x + end.width * 0.5f + xOffset,
                end.y,
                0
            );

            var startTan = startPos + Vector3.up * 50;
            var endTan = endPos + Vector3.down * 90;
            
            
            for (var i = 1; i < 4; i++) {
                var shadow = new Color32(visual.LineShadowColor.r,visual.LineShadowColor.g,visual.LineShadowColor.b,(byte)Mathf.Clamp(i*30f, 0, 255));
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, i*2);
            }
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, visual.LineColor, null, 3);
            DrawArrow(endPos, arrowTexture);
        }

        private void DrawArrow(Vector3 pos, Texture2D arrowTexture) {
            var color = GUI.color;
            GUI.color =  _mainVisual.LineColor; //Handles.xAxisColor;//
            var size = 14;
            GUI.DrawTexture(new Rect(pos.x - size/2, pos.y - 12, size, size), arrowTexture, ScaleMode.StretchToFill);
            GUI.color = color;
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