using System.Collections.Generic;
using System.Linq;
using Avocado.DependenciesVisualizer.Base.Editor.Scripts.Models;
using Avocado.DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor.Compilation;
using UnityEngine;

namespace Avocado.DependenciesVisualizer.Base.Editor.Scripts {
    public class DependencyManager {
        public IList<Node> Nodes { get; }
        public VisualizerState State { get; }
        public bool FirstRun { get; private set; }
        public List<Assembly> Assemblies { get; private set; }

        public DependencyManager() {
            Nodes = new List<Node>();
            State = new VisualizerState();
        }

        public void Initialize() {
            FirstRun = !State.LoadPreference();
            CreateNodes(State);
        }

        private void CreateNodes(VisualizerState state) {
            Assemblies = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies).ToList();
                //.Where(assembly => !assembly.name.Contains("Unity"));
            state.nodes.RemoveAll(data =>  Assemblies.FindAll(assembly => assembly.name == data.NodeId).Count == 0);
            
            foreach (var assembly in Assemblies) {
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

            UpdateNodeReferences();
        }

        public void UpdateNodeReferences() {
            foreach (var node in Nodes) {
                node.InjectOutputReferences(GetOutputNodes(node));
                node.InjectInputReferences(GetInputNodes(node));
            }
        }

        public Node AddNewNode(Vector2 position) {
            var assemblies = GetNotUsedAssemblies();
            if (assemblies.Count == 0) {
                return null;
            }

            var assembly = assemblies[0];
            var layer = State.layers[0];
            var data = new NodeData {
                NodeId = assembly.name,
                CurrentLayer = layer.Priority,
                CurrentLayerName = layer.Name,
                Position = position
            };
            State.nodes.Add(data);
            var node = new Node(assembly, data);
            Nodes.Add(node);
            State.ignoredAssemblies.RemoveAll(nodeData => nodeData.NodeId == data.NodeId);
            State.SavePreference();
            UpdateNodeReferences();

            return node;
        }

        public void DeleteNode(Node node) {
            State.nodes.Remove(node.Data);
            Nodes.Remove(node);
            if (State.ignoredAssemblies.FindAll(data => data.NodeId == node.Data.NodeId).Count == 0) {
                State.ignoredAssemblies.Add(node.Data);
            }

            UpdateNodeReferences();
            State.SavePreference();
        }

        public List<Assembly> GetNotUsedAssemblies() {
            return Assemblies.Where(assembly => !AssemblyIsVisualize(assembly)).ToList();
        }

        public bool AssemblyIsVisualize(Assembly assembly) {
            foreach (var node in Nodes) {
                if (node.Assembly == assembly) {
                    return true;
                }
            }

            return false;
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