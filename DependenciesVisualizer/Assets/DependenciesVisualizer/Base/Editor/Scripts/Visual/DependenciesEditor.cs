using System;
using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Core;
using DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual {
    public class DependenciesEditor : EditorWindow {
        [MenuItem("Dependencies Visualizer/Open")]
        public static void Open() {
            var editor = (DependenciesEditor) GetWindow(typeof(DependenciesEditor));
            editor.titleContent = new GUIContent("Dependencies Visualizer");
            editor.minSize = new Vector2(800, 600);
            editor.Show();
        }

        private Visualizer _visualizer;
        private static List<BaseNode> _nodes = new List<BaseNode>();
        private Vector3 _mousePosition;
        private bool _makeTransition;
        private bool _clickedOnWindow;
        private BaseNode _selectedNode;

        private void OnEnable() {
            _visualizer = new Visualizer();
            _nodes.Clear();
            var assemblies = _visualizer.GetAssemblies();

            var resString = string.Empty;
            foreach (var assembly in assemblies) {
                resString += assembly.name + "\n";
                foreach (var reference in assembly.assemblyReferences) {
                    resString += $"    -> {reference.name}\n";
                }
                resString += "------------------------------------- \n";
            }
            Debug.LogError(resString);

            foreach (var assembly in assemblies) {
                foreach (var reference in assembly.assemblyReferences) {
                }
            }
        }

        private void OnGUI() {
            var e = Event.current;
            _mousePosition = e.mousePosition;
            UserInput(e);
            DrawWindows();
        }

        private void DrawWindows() {
            BeginWindows();
            foreach (var node in _nodes) {
                node.DrawCurve();
            }

            for (var i = 0; i < _nodes.Count; i++) {
                _nodes[i].WindowRect = GUI.Window(i, _nodes[i].WindowRect, DrawNodeWindow, _nodes[i].WindowTitle);
                Debug.Log("draw " + i + "; " + _nodes[i].WindowRect);
            }

            EndWindows();
            
            void DrawNodeWindow(int index) {
                _nodes[index].DrawWindow();
                GUI.DragWindow();
            }
        }

        private void UserInput(Event e) {
            if (e.type == EventType.MouseDown && !_makeTransition) {
                switch (e.button) {
                    case 0:
                        LeftClick();
                        break;
                    case 1:
                        RightClick();
                        break;
                }
            }

            void RightClick() {
                _selectedNode = null;
                foreach (var node in _nodes) {
                    if (node.WindowRect.Contains(e.mousePosition)) {
                        _clickedOnWindow = true;
                        _selectedNode = node;
                        break;
                    }
                }

                if (_clickedOnWindow) {
                    ModifyNode(e);
                } else {
                    AddNewNode(e);
                }
            }

            void LeftClick() {
                
            }
        }

        private void AddNewNode(Event e) {
            var menu = new GenericMenu();
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add state"), false, ContextCallback,UserActions.AddState);
            menu.AddItem(new GUIContent("Add comment"), false, ContextCallback, UserActions.CommentNode);
            menu.ShowAsContext();
            e.Use();
        }

        private void ModifyNode(Event e) {
            
        }

        private void ContextCallback(object o) {
            var action = (UserActions) o;
            switch (action) {
                case UserActions.AddState:
                    var node = new StateNode {
                        WindowRect = new Rect(_mousePosition.x, _mousePosition.y, 150, 250),
                        WindowTitle = "State"
                    };
                    _nodes.Add(node);
                    break;
                case UserActions.CommentNode:

                    break;
                case UserActions.DeleteNode:

                    break;
                case UserActions.AddTransitionNode:

                    break;
            }
        }

        private void OnDisable() {
            
        }
    }
}
