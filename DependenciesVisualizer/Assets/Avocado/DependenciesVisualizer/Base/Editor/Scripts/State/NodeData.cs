using UnityEngine;

namespace Avocado.DependenciesVisualizer.Base.Editor.Scripts.State {
    [System.Serializable]
    public class NodeData {
        public string NodeId;
        public int CurrentLayer;
        public string CurrentLayerName;
        public Vector2 Position;
    }
}
