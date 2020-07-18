using System.Collections.Generic;
using System.Linq;
using DependenciesVisualizer.Base.Editor.Scripts.Commands;
using DependenciesVisualizer.Base.Editor.Scripts.Core;
using DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual {
    public class DependenciesEditor : EditorWindow {
        private  static DependenciesEditor _editor;
        private Visualizer _visualizer;
        private static List<BaseNode> _nodes = new List<BaseNode>();
        private Vector3 _mousePosition;
        private bool _makeTransition;
        private bool _clickedOnWindow;
        private BaseNode _selectedNode;
        private Stack<ICommand> _oldCommands = new Stack<ICommand>();
        
        [MenuItem("Dependencies Visualizer/Open")]
        public static void Open() {
            _editor = (DependenciesEditor) GetWindow(typeof(DependenciesEditor));
            _editor.titleContent = new GUIContent("Dependencies Visualizer");
            _editor.minSize = new Vector2(800, 600);
            _editor.Show();
            Init();
        }

        private static void Init() {
           // _visualizer = new Visualizer();
            _nodes.Clear();
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player).ToList();
            var nodePositionOffset = new Vector2(50, 50);
            foreach (var assembly in assemblies) {
                var newNode = new StateNode(assembly, nodePositionOffset, BaseNode.DefaultSize, assembly.name);
                _nodes.Add(newNode);
                
                nodePositionOffset = new Vector2(nodePositionOffset.x + BaseNode.DefaultSize.x + 50, nodePositionOffset.y);
                if (nodePositionOffset.x > _editor.minSize.x) {
                    nodePositionOffset = new Vector2(50, BaseNode.DefaultSize.y + 50);
                }
            }
        }

        private void OnEnable() {
          
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
            }

            EndWindows();
            
            void DrawNodeWindow(int index) {
                _nodes[index].DrawWindow();
                GUI.DragWindow();
            }
        }

        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor) {
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
            
            var shadow = new Color(0,0,0,0.006f);
            for (var i = 1; i < 4; i++) {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, i*0.5f);
            }
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 1);
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
                    NodeContextMenu(e);
                }
            }

            void LeftClick() {
                
            }
        }

        private void NodeContextMenu(Event e) {
            var menu = new GenericMenu();
         //   menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add state"), false, ContextCallback,UserActions.AddState);
            menu.AddItem(new GUIContent("Add comment"), false, ContextCallback, UserActions.CommentNode);
            menu.ShowAsContext();
            e.Use();
        }

        private void ModifyNode(Event e) {
            var menu = new GenericMenu();

            if (_selectedNode is StateNode stateNode) {
                var addTransitionName = "Add transition";
                if (stateNode.CurrentState is null) {
                    menu.AddDisabledItem(new GUIContent(addTransitionName));
                } else {
                    menu.AddItem(new GUIContent(addTransitionName), false, ContextCallback, UserActions.AddTransitionNode);
                }
            }

            menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.DeleteNode);
            if (_oldCommands.Count == 0) {
                menu.AddDisabledItem(new GUIContent("Undo"), false);
            } else {
                menu.AddItem(new GUIContent("Undo"), false, ContextCallback, UserActions.UndoLastAction);
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ContextCallback(object o) {
            var action = (UserActions) o;
            switch (action) {
                case UserActions.AddState:
                    //var node = new StateNode(_mousePosition, BaseNode.DefaultSize, "State");
                   // _nodes.Add(node);
                    break;
                case UserActions.CommentNode:
                    var node = new CommentNode(null, _mousePosition, BaseNode.DefaultSize, "Comment");
                     _nodes.Add(node);
                    break;
                case UserActions.DeleteNode:
                    if (_selectedNode != null) {
                        RemoveNode(_selectedNode);
                    }
                    break;
                case UserActions.AddTransitionNode:
                    if (_selectedNode is StateNode stateNode) {
                        var transition = stateNode.AddTransition();
                        AddTransitionNode(stateNode.CurrentState.Transitions.Count, transition, stateNode);
                    }
                    break;
                case UserActions.UndoLastAction:
                    UndoLastAction();
                    break;
            }
        }

        private static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from) {
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
        }

        private void RemoveNode(BaseNode node) {
            var command = new DeleteAssemblyCommand(node, _nodes);
            command.Execute();
            _oldCommands.Push(command);
        }

        private void UndoLastAction() {
            if (_oldCommands.Count == 0) {
                return;
            }
            
            _oldCommands.Pop().Undo();
        }

        private void OnDisable() {
            
        }
    }
}
