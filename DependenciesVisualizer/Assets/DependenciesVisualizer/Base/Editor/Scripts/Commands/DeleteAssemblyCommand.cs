using System.Collections.Generic;

namespace DependenciesVisualizer.Base.Editor.Scripts.Commands {
    public class DeleteAssemblyCommand : ICommand {
        private IList<Node> _container;
        private Node _node;
        
        public DeleteAssemblyCommand(Node node, IList<Node> container) {
            _container = container;
            _node = node;
        }

        public void Execute() {
            _node.Remove();
            _container.Remove(_node);
        }

        public void Undo() {
            _node.Restore();
            _container.Add(_node);
        }
    }
}