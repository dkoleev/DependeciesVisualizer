using System.Collections.Generic;
using Avocado.DependenciesVisualizer.Base.Editor.Scripts.State;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Avocado.DependenciesVisualizer.Base.Editor.Scripts {
    public class AssembliesWindow {
        private readonly List<AssemblyData> _assemblies;
        private readonly VisualizerState _state;
        private ReorderableList _layersList;
        private Rect _windowRect;
        private Assembly[] _systemAssemblies;
        private List<string> _systemAssemblyNames = new List<string>();
        
        public AssembliesWindow(VisualizerState state) {
            _state = state;
            _assemblies = _state.assemblies;
            _windowRect = new Rect(_state.LayersWindowPosition, new Vector2(330, 300));
            _systemAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);
            foreach (var assembly in _systemAssemblies) {
                _systemAssemblyNames.Add(assembly.name);
            }
        }
        
        public void Draw() {
            _windowRect.size = new Vector2(330, _assemblies.Count * 22 + 50);
            _state.LayersWindowPosition = _windowRect.position;
            _windowRect = GUI.Window(
                1001, 
                _windowRect,
                i => {
                    ReorderableListGUI.ListField(_assemblies, CustomListItem, DrawEmpty);
                    GUI.DragWindow();
                }, "Layers");
        }
        
        private AssemblyData CustomListItem(Rect position, AssemblyData itemValue) {
            if (itemValue is null) {
                itemValue = new AssemblyData();
                itemValue.Name = itemValue.Name;
            }
            
            EditorGUILayout.Popup("test", 0, _systemAssemblyNames.ToArray(), GUILayout.MaxWidth(100));
            
            //itemValue.Name = EditorGUI.TextField(position, itemValue.Name);
            
            position.x = 210;
            position.width = 75;
            
            return itemValue;
        }
                
        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
    }
}
