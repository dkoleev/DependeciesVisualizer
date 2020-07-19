using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Commands;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class MainWindow : EditorWindow {
        private  static MainWindow _editor;
        private Vector3 _mousePosition;
        private bool _makeTransition;
        private bool _clickedOnWindow;
        private Node _selectedNode;
        private Stack<ICommand> _oldCommands;
        private DependencyManager _manager;
        
        private List<string> _layers = new List<string>();

        [MenuItem("Dependencies Visualizer/Show")]
        public static void Open() {
            _editor = (MainWindow) GetWindow(typeof(MainWindow));
            _editor.titleContent = new GUIContent("Dependencies Visualizer");
            _editor.minSize = new Vector2(800, 600);
            _editor.Show();
        }
        
        private void OnEnable() {
            _manager = new DependencyManager();
            _manager.CreateNodes();
            _oldCommands = new Stack<ICommand>();
            
            SetNodePositionsByDependencies();
        }

        private void OnGUI() {
            var e = Event.current;
            _mousePosition = e.mousePosition;
            UserInput(e);
            DrawWindows();
            
            ReorderableListGUI.ListField(_layers, CustomListItem, DrawEmpty);
        }

        private void DrawWindows() {
            BeginWindows();
            DrawNodes();
            EndWindows();
        }

        private void DrawNodes() {
            for (var i = 0; i < _manager.Nodes.Count; i++) {
                _manager.Nodes[i].Draw(i);
                _manager.Nodes[i].DrawOutputReferences();
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
                _clickedOnWindow = false;
                foreach (var node in _manager.Nodes) {
                    if (node.WindowRect.Contains(e.mousePosition)) {
                        _clickedOnWindow = true;
                        _selectedNode = node;
                        break;
                    }
                }

                if (_clickedOnWindow) {
                    ModifyNode(e);
                } else {
                    NodeContextMenu(e);
                }
            }

            void LeftClick() {
                
            }
        }
        
         private void NodeContextMenu(Event e) {
            var menu = new GenericMenu();
         //   menu.AddSeparator("");
            menu.AddItem(new GUIContent("Create Assembly"), false, ContextCallback,UserActions.AddNode);
            if (_oldCommands.Count == 0) {
                menu.AddDisabledItem(new GUIContent("Undo"), false);
            } else {
                menu.AddItem(new GUIContent("Undo"), false, ContextCallback, UserActions.UndoLastAction);
            }
            
            menu.ShowAsContext();
            e.Use();
        }

        private void ModifyNode(Event e) {
            var menu = new GenericMenu();
            if (_selectedNode is Node stateNode) {
                var addTransitionName = "Add dependency";
                menu.AddItem(new GUIContent(addTransitionName), false, ContextCallback, UserActions.AddDependency);
            }

            menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.DeleteNode);

            menu.ShowAsContext();
            e.Use();
        }

        private void ContextCallback(object o) {
            var action = (UserActions) o;
            switch (action) {
                case UserActions.AddNode:
                    //var node = new StateNode(_mousePosition, BaseNode.DefaultSize, "State");
                   // _nodes.Add(node);
                    break;
                case UserActions.DeleteNode:
                    if (_selectedNode != null) {
                        RemoveNode(_selectedNode);
                    }
                    break;
                case UserActions.AddDependency:
                   // AddDependency(stateNode.CurrentState.Transitions.Count, transition, stateNode);
                    break;
                case UserActions.UndoLastAction:
                    UndoLastAction();
                    break;
            }
        }
        
        /*private static void AddDependency(int index, Node from, Node to) {
            var fromRect = from.WindowRect;
            fromRect.x += 50;
            var targetY = fromRect.y - fromRect.height;
            if (!(from.CurrentState is null)) {
                targetY += index * 100;
            }

            fromRect.y = targetY;
            var transitionNode = CreateInstance<TransitionNode>();
            transitionNode.Init(from, transition);
            transitionNode.WindowRect = new Rect(fromRect.x + 200 + 100, fromRect.y + (fromRect.height * 0.7f), 200, 80);
            transitionNode.WindowTitle = "Condition check";
            _nodes.Add(transitionNode);

            return transitionNode;
        }*/

        private void SetNodePositionsByDependencies() {
            var _amountOnLevel = new Dictionary<int, int>();
            
            foreach (var node in _manager.Nodes) {
                var level = node.GetDependencyLevel(0);
                if (!_amountOnLevel.ContainsKey(level)) {
                    _amountOnLevel.Add(level, 0);
                }

                node.SetPosition(new Vector2(50 + _amountOnLevel[level] * 350, 50 + level * 160));
                _amountOnLevel[level]++;
            }
        }
        
        
        private string CustomListItem(Rect position, string itemValue) {
            // Text fields do not like null values!
            if (itemValue == null)
                itemValue = "";
            return EditorGUI.TextField(position, itemValue);
        }

        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
        
        private void RemoveNode(Node node) {
            var command = new DeleteAssemblyCommand(node, _manager.Nodes);
            command.Execute();
            _oldCommands.Push(command);
        }
        
        private void UndoLastAction() {
            if (_oldCommands.Count == 0) {
                return;
            }
            
            _oldCommands.Pop().Undo();
        }
    }
}