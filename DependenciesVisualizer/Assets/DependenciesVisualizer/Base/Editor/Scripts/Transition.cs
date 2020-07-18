using DependenciesVisualizer.Base.Editor.Scripts.Behaviour;
using DependenciesVisualizer.Base.Editor.Scripts.Behaviour.Conditions;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public class Transition {
        public Condition Condition;
        public State TargetState;
        public bool Disable;
    }
}
