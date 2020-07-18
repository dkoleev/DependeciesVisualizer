namespace DependenciesVisualizer.Base.Editor.Scripts.Commands {
    public interface ICommand {
        void Execute();
        void Undo();
    }
}
