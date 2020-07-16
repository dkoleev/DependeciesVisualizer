using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Avocado.UnityToolbox.ArchitectureVisualizer.Editor {
    public class Visualizer
    {
        public List<string> FindAsmdefs() {
            string[] guids = AssetDatabase.FindAssets("t:asmdef", new[] {"Assets"});

            return guids.ToList();
        }
    }
}
