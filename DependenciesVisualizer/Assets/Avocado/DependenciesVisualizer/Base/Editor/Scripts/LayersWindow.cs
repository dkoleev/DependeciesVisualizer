using System.Collections.Generic;
using Avocado.DependenciesVisualizer.Base.Editor.Scripts.State;
using DependenciesVisualizer.Base.Editor.Scripts.ReorderList;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Avocado.DependenciesVisualizer.Base.Editor.Scripts {
    public class LayersWindow {
        public const string DefaultLayerName = "Default";
        public List<LayerData> Layers => _layers;
        private readonly List<LayerData> _layers;
        private readonly VisualizerState _state;
        private ReorderableList _layersList;
        private Rect _windowRect;

        public LayersWindow(VisualizerState state) {
            _state = state;
            _layers = _state.layers;
            _windowRect = new Rect(_state.LayersWindowPosition, new Vector2(330, 300));
        }

        public void Draw() {
            _windowRect.size = new Vector2(330, _layers.Count * 22 + 50);
            _state.LayersWindowPosition = _windowRect.position;
            _windowRect = GUI.Window(
                1000, 
                _windowRect,
                i => {
                    ReorderableListGUI.ListField(_layers, CustomListItem, DrawEmpty);
                    GUI.DragWindow();
                }, "Layers");
        }
        
        private LayerData CustomListItem(Rect position, LayerData itemValue) {
            var getPriorityByIndex = _layers.FindIndex(test => test == itemValue);
            
            if (itemValue is null) {
                itemValue = new LayerData();
                itemValue.Name = "Default_" + getPriorityByIndex;
                itemValue.Color = _state.layerDefaultColor;
            }
            
            position.x = 30;
            position.width = 25;
            EditorGUI.LabelField(position, getPriorityByIndex.ToString());
            itemValue.Priority = getPriorityByIndex;
            
            /*position.x = 50;
            position.width = 50;
            EditorGUI.LabelField(position, "Name");*/

            position.width -= 50;
            position.x = 50;
            position.xMax = 200;
            itemValue.Name = EditorGUI.TextField(position, itemValue.Name);
            
            position.x = 210;
            position.width = 75;
            itemValue.Color = EditorGUI.ColorField(position, itemValue.Color);
            _state.layerDefaultColor = itemValue.Color;
            
            return itemValue;
        }
                
        private void DrawEmpty() {
            GUILayout.Label("No items in list.", EditorStyles.miniLabel);
        }
    }
}
