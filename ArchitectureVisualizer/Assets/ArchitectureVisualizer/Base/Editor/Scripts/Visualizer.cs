using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using Object = System.Object;

namespace ArchitectureVisualizer.Base.Editor.Scripts {
    public class Visualizer
    {
        public List<string> FindAssets() {
            var data = new List<Object>();
            var guids = AssetDatabase.FindAssets("t:asmdef", new[] {"Assets"}).ToList();
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
            }

            return guids.ToList();
        }
        
        public List<Assembly> GetAssemblies() {
            var playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);

            return playerAssemblies.ToList();
        }
    }
}
