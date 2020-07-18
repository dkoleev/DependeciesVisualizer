using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Behaviour.Conditions {
    [CreateAssetMenu(menuName = "Conditions/Is Dead")]
    public class IsDead : Condition {
        public override bool CheckCondition(StateManager stateManager) {
            return stateManager.Health <= 0;
        }
    }
}
