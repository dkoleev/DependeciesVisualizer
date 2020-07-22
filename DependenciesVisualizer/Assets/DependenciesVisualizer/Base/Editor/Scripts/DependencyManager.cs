using System.Collections.Generic;
using System.Linq;
using DependenciesVisualizer.Base.Editor.Scripts.Models;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class DependencyManager {
        public IList<Node> Nodes { get; }
        
        public DependencyManager() {
            Nodes = new List<Node>();
        }

        public ICollection<Node> CreateNodes(LayersWindow layersWindow, VisualizerState state) {
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies).
                Where(assembly => !assembly.name.Contains("Unity"));
            
            foreach (var assembly in assemblies) {
                var data = state.nodes.FirstOrDefault(nodeData => nodeData.NodeId == assembly.name);
                if (data is null) {
                    var layer = layersWindow.Layers[0];
                    data = new NodeData {
                        NodeId = assembly.name,
                        CurrentLayer = layer.Priority,
                        CurrentLayerName = layer.Name,
                        Position = Vector2.zero
                    };
                }

                Nodes.Add(new Node(assembly, data));
            }

            foreach (var node in Nodes) {
                node.InjectInputReferences(GetInputNodes(node));
                node.InjectOutputReferences(GetOutputNodes(node));
            }

            return Nodes;
        }

        private IList<Node> GetOutputNodes(Node nodeView) {
            var result = new List<Node>();
            
            foreach (var node1 in Nodes) {
                if (node1.IsDependent(nodeView)) {
                    result.Add(node1);
                }
            }

            return result;
        }

        private IList<Node> GetInputNodes(Node nodeView) {
            var result = new List<Node>();
            
            foreach (var node1 in Nodes) {
                if (nodeView.IsDependent(node1)) {
                    result.Add(node1);
                }
            }
            
            return result;
        }
    }
}