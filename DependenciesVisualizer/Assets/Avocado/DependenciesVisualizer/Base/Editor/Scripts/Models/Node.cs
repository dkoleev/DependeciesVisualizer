using System.Collections.Generic;
using System.Linq;
using Avocado.DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor.Compilation;

namespace Avocado.DependenciesVisualizer.Base.Editor.Scripts.Models {
    public class Node {
        public Assembly Assembly { get; private set; }

        public NodeData Data => _data;
        public IList<Node> OutputDependencies { get; private set; }
        public IList<Node> InputDependencies{ get; private set; }
        
        private NodeData _data;
        
        public Node(Assembly assembly, NodeData data) {
            Assembly = assembly;
            _data = data;
            OutputDependencies = new List<Node>();
            InputDependencies = new List<Node>();
        }

        public void SetAssembly(Assembly assembly) {
            Assembly = assembly;
            _data.NodeId = assembly.name;
        }

        public void InjectOutputReferences(IList<Node> references) {
            OutputDependencies = references;
        }
        
        public void InjectInputReferences(IList<Node> references) {
            InputDependencies = references;
        }
        
        
        public bool HaveOutputDependencies() {
            return OutputDependencies.Count > 0;
        }
        
        public bool HaveInputDependencies() {
            return InputDependencies.Count > 0;
        }

        public bool IsOutput(Node nodeView) {
            return OutputDependencies.Contains(nodeView);
        }
        
        public bool IsInput(Node nodeView) {
            return InputDependencies.Contains(nodeView);
        }
        
        public bool IsDependent(Node node) {
            return Assembly.assemblyReferences.Contains(node.Assembly);
        }

        public bool HaveCycleReferences() {
            return false;
        }

        public bool IsCorrectDependency(Node node) {
            return Data.CurrentLayer >= node.Data.CurrentLayer;
        }

        public int GetDependencyLevel(int startLevel) {
            if (HaveOutputDependencies()) {
                startLevel++;
                var bufLevel = startLevel;
                foreach (var dependency in OutputDependencies) {
                    var depLevel = dependency.GetDependencyLevel(startLevel);
                    if (depLevel > bufLevel) {
                        bufLevel = depLevel;
                    }
                }

                startLevel = bufLevel;
            }

            return startLevel;
        }
    }
}