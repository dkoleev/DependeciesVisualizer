#pragma warning disable CS0649
using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Commands;
using DependenciesVisualizer.Base.Editor.Scripts.Models;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class MainWindow : EditorWindow {
        [SerializeField] private Texture2D _arrowTextureUp;
        [SerializeField] private Texture2D _arrowTextureDown;
        [SerializeField] private Texture2D _arrowTextureLeft;
        [SerializeField] private Texture2D _arrowTextureRight;

        public Texture2D ArrowUp => _arrowTextureUp;
        public Texture2D ArrowDown => _arrowTextureDown;
        public Texture2D ArrowLeft => _arrowTextureLeft;
        public Texture2D ArrowRight => _arrowTextureRight;

        private IList<NodeView> _nodeViews;
        private  static MainWindow _editor;
        private Vector3 _mousePosition;
        private bool _makeTransition;
        private bool _clickedOnWindow;
        private NodeView _selectedNodeView;
        private Stack<ICommand> _oldCommands;
        private DependencyManager _manager;
        private LayersWindow _layersWindow;
        private static Vector2 _windowMinSize = new Vector2(800, 600);

        [MenuItem("Dependencies Visualizer/Show")]
        public static void Open() {
            _editor = (MainWindow) GetWindow(typeof(MainWindow));
            _editor.titleContent = new GUIContent("Dependencies Visualizer");
            _editor.minSize = _windowMinSize;
            _editor.Show();
        }
        
        private void OnEnable() {
            _nodeViews = new List<NodeView>();
            _manager = new DependencyManager();
            _manager.Initialize();
          
            bool needSortNodes = _manager.State.nodes.Count == 0;

            _layersWindow = new LayersWindow(_manager.State);
            foreach (var node in _manager.Nodes) {
                _nodeViews.Add(new NodeView(this, _layersWindow, node));
            }
            
            _oldCommands = new Stack<ICommand>();

            if (needSortNodes) {
                SetNodePositionsByDependencies();
            }
        }

        private void OnDisable() {
            _manager.State.SavePreference();
        }

        private void OnGUI() {
            var e = Event.current;
            _mousePosition = e.mousePosition;
            UserInput(e);
            DrawWindows();
        }

        private void DrawWindows() {
            BeginWindows();
            DrawNodes();
            _layersWindow.Draw();
            EndWindows();
        }

        private void DrawNodes() {
            for (var i = 0; i < _nodeViews.Count; i++) {
                _nodeViews[i].Draw(i);
                _nodeViews[i].DrawOutputReferences();
            }
        }

        private void UserInput(Event e) {
            if (e.type == EventType.MouseDown) {
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
                _selectedNodeView = null;
                _clickedOnWindow = false;
                foreach (var node in _nodeViews) {
                    if (node.WindowRect.Contains(e.mousePosition)) {
                        _clickedOnWindow = true;
                        _selectedNodeView = node;
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
            if (_selectedNodeView is NodeView stateNode) {
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
                    if (_selectedNodeView != null) {
                        RemoveNode(_selectedNodeView);
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

        private void SetNodePositionsByDependencies() {
            var _amountOnLevel = new Dictionary<int, int>();
            
            foreach (var node in _nodeViews) {
                var level = node.Model.GetDependencyLevel(0);
                if (!_amountOnLevel.ContainsKey(level)) {
                    _amountOnLevel.Add(level, 0);
                }

                var minPosition = 150;
                var maxPosition = _windowMinSize.x - 100;
                var posY = maxPosition - (level * 160);

                if (posY < minPosition) {
                    posY = minPosition - level * 20;
                    if (posY < 50) {
                        posY = 50;
                    }
                }
              
                node.SetPosition(new Vector2(50 + _amountOnLevel[level] * 350, posY)); //-50 + level * 160
                _amountOnLevel[level]++;
            }
        }

        private void RemoveNode(NodeView nodeView) {
            var command = new DeleteAssemblyCommand(nodeView, _nodeViews);
            command.Execute();
            _oldCommands.Push(command);
        }

        public NodeView GetNodeViewByModel(Node node) {
            foreach (var view in _nodeViews) {
                if (view.Model == node) {
                    return view;
                }
            }

            return null;
        }

        private void UndoLastAction() {
            if (_oldCommands.Count == 0) {
                return;
            }
            
            _oldCommands.Pop().Undo();
        }
    }
}