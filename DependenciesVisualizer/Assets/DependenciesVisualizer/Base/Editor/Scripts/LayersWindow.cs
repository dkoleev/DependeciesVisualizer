using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class LayersWindow {
        public const string DefaultLayerName = "Default";
        public List<LayerData> Layers => _layers;
        private readonly List<LayerData> _layers;
        private readonly VisualizerState _state;
        private ReorderableList _layersList;

        public LayersWindow(VisualizerState state) {
            _state = state;
            _layers = _state.layers;
        }

        public void Draw() {
            ReorderableListGUI.ListField<LayerData>(_layers, CustomListItem, DrawEmpty);
        }
        
        private LayerData CustomListItem(Rect position, LayerData itemValue) {
            var getPriorityByIndex = _layers.FindIndex(test => test == itemValue);
            
            if (itemValue is null) {
                itemValue = new LayerData();
                itemValue.Name = "Default_" + getPriorityByIndex;
                itemValue.Color = _state.layerDefaultColor;
            }
            
            position.x = 50;
            position.width = 50;
            EditorGUI.LabelField(position, "Name");
            

            position.width -= 50;
            position.x = 100;
            position.xMax = 300;
            itemValue.Name = EditorGUI.TextField(position, itemValue.Name);
            
            position.x = 350;
            position.width = 100;
            EditorGUI.LabelField(position, "Priority: " + getPriorityByIndex);
            itemValue.Priority = getPriorityByIndex;
            
            position.x = 450;
            position.width = 100;
            itemValue.Color = EditorGUI.ColorField(position, itemValue.Color);
            _state.layerDefaultColor = itemValue.Color;
            
            return itemValue;
        }
                
        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
    }
}
