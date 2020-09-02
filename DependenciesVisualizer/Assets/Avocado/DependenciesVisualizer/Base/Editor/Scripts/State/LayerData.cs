using UnityEngine;

namespace Avocado.DependenciesVisualizer.Base.Editor.Scripts.State {
    [System.Serializable]
    public class LayerData {
        public string Name;
        public int Priority;
        public Color Color;
    }
    
    [System.Serializable]
    public class AssemblyData {
        public string Name;
    }
}