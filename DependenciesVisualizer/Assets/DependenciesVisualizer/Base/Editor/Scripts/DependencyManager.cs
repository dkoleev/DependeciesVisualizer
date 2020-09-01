using System.Collections.Generic;
using System.Linq;
using DependenciesVisualizer.Base.Editor.Scripts.Models;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class DependencyManager {
        public const string NodeEmptyId = "None";
        public IList<Node> Nodes { get; }
        public VisualizerState State { get; }
        public bool FirstRun { get; private set; }

        public DependencyManager() {
            Nodes = new List<Node>();
            State = new VisualizerState();
        }

        public void Initialize() {
            FirstRun = !State.LoadPreference();
            CreateNodes(State);
        }

        private void CreateNodes(VisualizerState state) {
            var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies).ToList();
                //.Where(assembly => !assembly.name.Contains("Unity"));
            state.nodes.RemoveAll(data =>  assemblies.FindAll(assembly => assembly.name == data.NodeId).Count == 0 &&
                                           data.NodeId != NodeEmptyId);
            
            foreach (var assembly in assemblies) {
                var data = state.nodes.FirstOrDefault(nodeData => nodeData.NodeId == assembly.name);
                if (data is null) {
                    var layer = state.layers[0];
                    data = new NodeData {
                        NodeId = assembly.name,
                        CurrentLayer = layer.Priority,
                        CurrentLayerName = layer.Name,
                        Position = Vector2.zero
                    };
                    state.nodes.Add(data);
                }

                if (state.ignoredAssemblies.FindAll(nodeData => nodeData.NodeId == data.NodeId).Count == 0) {
                    Nodes.Add(new Node(assembly, data));
                }
            }
            foreach (var node in Nodes) {
                node.InjectOutputReferences(GetOutputNodes(node));
                node.InjectInputReferences(GetInputNodes(node));
            }

            var noneNodes = state.nodes.FindAll(data => data.NodeId == NodeEmptyId);
            foreach (var noneNode in noneNodes) {
                Nodes.Add(new Node(null, noneNode));
            }
        }

        public Node AddNewNode(Vector2 position) {
            var layer = State.layers[0];
            var data = new NodeData {
                NodeId = NodeEmptyId,
                CurrentLayer = layer.Priority,
                CurrentLayerName = layer.Name,
                Position = position
            };
            State.nodes.Add(data);
            var node = new Node(null, data);
            Nodes.Add(node);

            return node;
        }

        public void DeleteNode(Node node) {
            State.nodes.Remove(node.Data);
            Nodes.Remove(node);
            if (node.Data.NodeId != NodeEmptyId && State.ignoredAssemblies.FindAll(data => data.NodeId == node.Data.NodeId).Count == 0) {
                State.ignoredAssemblies.Add(node.Data);
            }
        }

        private IList<Node> GetInputNodes(Node node) {
            var result = new List<Node>();
            
            foreach (var node1 in Nodes) {
                if (node1.IsDependent(node)) {
                    result.Add(node1);
                }
            }

            return result;
        }

        private IList<Node> GetOutputNodes(Node node) {
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