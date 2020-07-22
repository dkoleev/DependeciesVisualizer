using System.Collections.Generic;
using System.Linq;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor.Compilation;

namespace DependenciesVisualizer.Base.Editor.Scripts.Models {
    public class Node {
        public Assembly Assembly { get; }

        public NodeData Data => _data;
        public IList<Node> InputDependencies { get; private set; }
        public IList<Node> OutputDependencies{ get; private set; }
        
        private NodeData _data;
        
        public Node(Assembly assembly, NodeData data) {
            Assembly = assembly;
            _data = data;
        }
        
        public void InjectInputReferences(IList<Node> references) {
            InputDependencies = references;
        }
        
        public void InjectOutputReferences(IList<Node> references) {
            OutputDependencies = references;
        }
        
        
        public bool HaveInputDependencies() {
            return InputDependencies.Count > 0;
        }
        
        public bool HaveOutputDependencies() {
            return OutputDependencies.Count > 0;
        }

        public bool IsInput(Node nodeView) {
            return InputDependencies.Contains(nodeView);
        }
        
        public bool IsOutput(Node nodeView) {
            return OutputDependencies.Contains(nodeView);
        }
        
        public bool IsDependent(Node node) {
            return Assembly.assemblyReferences.Contains(node.Assembly);
        }
        
        public int GetDependencyLevel(int startLevel) {
            if (HaveInputDependencies()) {
                startLevel++;
                var bufLevel = startLevel;
                foreach (var dependency in InputDependencies) {
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