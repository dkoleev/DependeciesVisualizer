using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class LayersWindow {
        private List<LayerData> _layers = new List<LayerData>();

        public void Draw() {
            ReorderableListGUI.ListField<LayerData>(_layers, CustomListItem, DrawEmpty);
        }
        
        private LayerData CustomListItem(Rect position, LayerData itemValue) {
            var getPriorityByIndex = _layers.FindIndex(test => test == itemValue);
            
            if (itemValue is null) {
                itemValue = new LayerData();
                itemValue.Name = "Default_" + getPriorityByIndex;
                itemValue.Color = Color.black;
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
            
            position.x = 450;
            position.width = 100;
            itemValue.Color = EditorGUI.ColorField(position, itemValue.Color);
            
            return itemValue;
        }
                
        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
        
    }

    public class LayerData {
        public string Name;
        public string Priority;
        public Color Color;
    }
}
