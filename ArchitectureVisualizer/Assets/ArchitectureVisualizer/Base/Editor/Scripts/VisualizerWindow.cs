using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ArchitectureVisualizer.Base.Editor.Scripts {
    public class VisualizerWindow : EditorWindow {
        [MenuItem("Architecture Visualizer/Open")]
        public static void Open() {
            var window = (VisualizerWindow) GetWindow(typeof(VisualizerWindow));
            window.titleContent = new GUIContent("Architecture Visualizer");
            window.Show();
        }

        private Visualizer _visualizer;

        private void OnEnable() {
            _visualizer = new Visualizer();
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

        private void OnDisable() {
            
        }
    }
}
