using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DependenciesVisualizer.Base.Editor.Scripts.Behaviour {
    [CreateAssetMenu]
    public class State : ScriptableObject {
        [FormerlySerializedAs("_transitions")]
        public List<Transition> Transitions = new List<Transition>();
        
        public void Tick() {
            
        }

        public Transition AddTransition() {
            var transition = new Transition();
            Transitions.Add(transition);

            return transition;
        }
    }
}
