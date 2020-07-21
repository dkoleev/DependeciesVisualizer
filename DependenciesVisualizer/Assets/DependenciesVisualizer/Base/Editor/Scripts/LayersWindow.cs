using System;
using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor;
using UnityEngine;
using ReorderableList = UnityEditorInternal.ReorderableList;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class LayersWindow {
        public const string DefaultLayerName = "Default";
        public List<LayerData> Layers => _layers;
        private List<LayerData> _layers = new List<LayerData>();
        private VisualizerPreferences _preferences;
        
        private ReorderableList _layersList;
        private ReorderableList list1;

        public LayersWindow(VisualizerPreferences preferences) {
            _preferences = preferences;
            _layers = _preferences.layers;
        }

        public void Draw() {
            ReorderableListGUI.ListField<LayerData>(_layers, CustomListItem, DrawEmpty);
        }
        
        private LayerData CustomListItem(Rect position, LayerData itemValue) {
            var getPriorityByIndex = _layers.FindIndex(test => test == itemValue);
            
            if (itemValue is null) {
                itemValue = new LayerData();
                itemValue.Name = "Default_" + getPriorityByIndex;
                itemValue.Color = _preferences.layerDefaultColor;
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
            _preferences.layerDefaultColor = itemValue.Color;
            
            return itemValue;
        }
                
        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
    }
}
