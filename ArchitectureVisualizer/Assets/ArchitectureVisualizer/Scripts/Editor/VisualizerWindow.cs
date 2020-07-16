using System;
using UnityEditor;
using UnityEngine;

namespace Avocado.UnityToolbox.ArchitectureVisualizer.Editor {
    public class VisualizerWindow : EditorWindow {
        [MenuItem("Architecture Visualizer/Open")]
        public static void Open() {
            var window = (VisualizerWindow) GetWindow(typeof(VisualizerWindow));
            window.titleContent = new GUIContent("Architecture Visualizer");
            window.Show();
        }

        private Visualizer _visualizer;

        private void OnEnable() {
            Debug.LogError("enable");
            _visualizer = new Visualizer();
            var asmdefs = _visualizer.FindAsmdefs();
            foreach (var asmdef in asmdefs) {
                Debug.LogError(asmdef);
            }
        }

        private void OnDisable() {
            
        }
    }
}
