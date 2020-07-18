using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Behaviour.Conditions {
    public abstract class Condition : ScriptableObject {
        public abstract bool CheckCondition(StateManager stateManager);
    }
}