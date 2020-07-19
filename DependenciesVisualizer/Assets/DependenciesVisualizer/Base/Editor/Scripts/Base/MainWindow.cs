using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Commands;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
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
            
        }

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
    }
}