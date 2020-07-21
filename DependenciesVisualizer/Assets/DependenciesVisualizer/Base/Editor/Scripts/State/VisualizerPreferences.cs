using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.State {
    [System.Serializable]
    public class VisualizerPreferences {
        /*public bool showBrushSize = true;
        public int brushDetail = 18;
        public float minBrushDrawSize = 0.5f;
        public Color brushColor = new Color(24f / 255f, 118f / 255f, 175f / 255f);

        public bool showCentralDot = true;
        public float dotSize = 0.1f;

        public bool showNormal = true;*/
        public Color layerDefaultColor;

        public List<LayerData> layers;
        public List<NodeData> nodes;
        //  public bool autoSave;

        public VisualizerPreferences() {
            layerDefaultColor = new Color(147f / 255f, 244f / 255f, 66f / 255f);
            layers = new List<LayerData>();
            nodes = new List<NodeData>();
        }

        public void LoadDefaults() {
            layers.Add(
                new LayerData {
                    Name = LayersWindow.DefaultLayerName,
                    Color = layerDefaultColor,
                    Priority = 0
                });
        }

        public bool LoadPreference() {
            VisualizerPreferences objXml = null;

            string[] guids = AssetDatabase.FindAssets("SceneEditorPref", null);
            if (guids.Length > 0) {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);

                Stream fs = new FileStream(path, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(VisualizerPreferences));

                objXml = (VisualizerPreferences) serializer.Deserialize(fs);
                fs.Close();
            }

            if (objXml != null) {
                layerDefaultColor = objXml.layerDefaultColor;
                layers = objXml.layers;
                nodes = objXml.nodes;

                return true;
            } else {
                LoadDefaults();
            }

            SavePreference();
            return false;
        }

        public void SavePreference() {
            string[] guids = AssetDatabase.FindAssets("VisualizerPreferences", null);
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            path = Path.GetDirectoryName(path) + "/SceneEditorPref.xml";
            Stream fs = new FileStream(path, FileMode.Create);
            XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
            XmlSerializer serializer = new XmlSerializer(typeof(VisualizerPreferences));
            serializer.Serialize(writer, this);
            writer.Close();

            AssetDatabase.Refresh();
        }
    }
}