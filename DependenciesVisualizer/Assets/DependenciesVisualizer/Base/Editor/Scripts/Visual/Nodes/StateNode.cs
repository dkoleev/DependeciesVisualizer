using DependenciesVisualizer.Base.Editor.Scripts.Behaviour;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes {
    public class StateNode : BaseNode {
        public State CurrentState;
        private bool _collapse;
        
        public StateNode(Assembly assembly, Vector2 position, Vector2 size, string title) : base(assembly, position, size, title) { }

        public override void DrawWindow() {
            if (CurrentState is null) {
                EditorGUILayout.LabelField("Add state to modify");
            } 
            
            WindowRect.height = _collapse ? 75 : 150;
            _collapse = EditorGUILayout.Toggle(" ", _collapse);

            CurrentState = (State)EditorGUILayout.ObjectField(CurrentState, typeof(State), false);
        }

        public override void DrawCurve() {
            base.DrawCurve();
        }

        public Transition AddTransition() {
            return CurrentState.AddTransition();
        }
    }
}
