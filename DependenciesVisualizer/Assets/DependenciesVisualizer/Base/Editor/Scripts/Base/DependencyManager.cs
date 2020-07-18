using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Base {
    public class DependencyManager {
        public IList<Node> Nodes { get; }

        public DependencyManager() {
            Nodes = new List<Node>();
            Nodes = new List<Node>();
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            
            foreach (var assembly in assemblies) {
                Nodes.Add(new Node(assembly, Vector2.zero));
            }

            foreach (var node in Nodes) {
                node.InjectReferences(GetInputNodes(node));
            }
        }

        public IList<Node> GetInputNodes(Node node) {
            var result = new List<Node>();
            
            foreach (var node1 in Nodes) {
                if (node1.IsDependent(node)) {
                    result.Add(node1);
                }
            }

            return result;
        }

        public IList<Node> GetOutputNodes(Node node) {
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