using System.Collections.Generic;
using DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes;

namespace DependenciesVisualizer.Base.Editor.Scripts.Commands {
    public class DeleteAssemblyCommand : ICommand {
        private IList<BaseNode> _container;
        private BaseNode _node;
        
        public DeleteAssemblyCommand(BaseNode node, IList<BaseNode> container) {
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