using System.Collections.Generic;
using System.Linq;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class DependencyManager {
        public IList<Node> Nodes { get; }
        
        private List<NodeData> _nodesData = new List<NodeData>();

        public DependencyManager() {
            Nodes = new List<Node>();
        }

        public void CreateNodes(LayersWindow layersWindow, VisualizerPreferences preferences) {
            _nodesData = preferences.nodes;
            
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies).
                Where(assembly => !assembly.name.Contains("Unity"));
            
            foreach (var assembly in assemblies) {
                var data = _nodesData.FirstOrDefault(nodeData => nodeData.NodeId == assembly.name);
                if (data is null) {
                    var layer = layersWindow.Layers[0];
                    data = new NodeData {
                        NodeId = assembly.name,
                        CurrentLayer = layer.Priority,
                        CurrentLayerName = layer.Name
                    };
                    _nodesData.Add(data);
                }

                Nodes.Add(new Node(assembly, Vector2.zero, layersWindow, data));
            }

            foreach (var node in Nodes) {
                node.InjectInputReferences(GetInputNodes(node));
                node.InjectOutputReferences(GetOutputNodes(node));
            }
        }

        private IList<Node> GetOutputNodes(Node node) {
            var result = new List<Node>();
            
            foreach (var node1 in Nodes) {
                if (node1.IsDependent(node)) {
                    result.Add(node1);
                }
            }

            return result;
        }

        private IList<Node> GetInputNodes(Node node) {
            var result = new List<Node>();
            
            foreach (var node1 in Nodes) {
                if (node.IsDependent(node1)) {
                    result.Add(node1);
                }
            }
            
            return result;
        }
    }
}