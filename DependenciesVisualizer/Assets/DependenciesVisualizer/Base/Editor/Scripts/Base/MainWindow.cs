using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Commands;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Base {
    public class MainWindow : EditorWindow {
        private  static MainWindow _editor;
        private Vector3 _mousePosition;
        private bool _makeTransition;
        private bool _clickedOnWindow;
        private Stack<ICommand> _oldCommands;
        private DependencyManager _manager;
        
        [MenuItem("Dependencies Visualizer/Show")]
        public static void Open() {
            _editor = (MainWindow) GetWindow(typeof(MainWindow));
            _editor.titleContent = new GUIContent("Dependencies Visualizer");
            _editor.minSize = new Vector2(800, 600);
            _editor.Show();
        }
        
        private void OnEnable() {
            _manager = new DependencyManager();
            _oldCommands = new Stack<ICommand>();
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
            EndWindows();
        }

        private void DrawNodes() {
            for (var i = 0; i < _manager.Nodes.Count; i++) {
                _manager.Nodes[i].Draw(i);
                _manager.Nodes[i].DrawInputReferences();
            }
        }

        private void UserInput(Event e) {
            
        }
    }
}