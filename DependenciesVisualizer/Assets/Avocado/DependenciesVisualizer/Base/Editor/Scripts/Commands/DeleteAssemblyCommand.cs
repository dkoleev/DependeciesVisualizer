using System.Collections.Generic;
using Avocado.DependenciesVisualizer.Base.Editor.Scripts;

namespace DependenciesVisualizer.Base.Editor.Scripts.Commands {
    public class DeleteAssemblyCommand : ICommand {
        private IList<NodeView> _container;
        private NodeView _nodeView;
        
        public DeleteAssemblyCommand(NodeView nodeView, IList<NodeView> container) {
            _container = container;
            _nodeView = nodeView;
        }

        public void Execute() {
            _nodeView.Remove();
            _container.Remove(_nodeView);
        }

        public void Undo() {
            _nodeView.Restore();
            _container.Add(_nodeView);
        }
    }
}