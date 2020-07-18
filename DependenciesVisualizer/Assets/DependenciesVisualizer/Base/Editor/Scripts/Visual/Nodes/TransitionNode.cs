using DependenciesVisualizer.Base.Editor.Scripts.Behaviour.Conditions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts.Visual.Nodes {
    public class TransitionNode : BaseNode {
        public Transition TargetTransition;
        public StateNode EnterState;
        public StateNode TargetState;
        
        public TransitionNode(Assembly assembly, Vector2 position, Vector2 size, string title) : base(assembly, position, size, title) { }

        public void Init(StateNode enterState, Transition transition) {
            EnterState = enterState;
            TargetTransition = transition;
        }

        public override void DrawWindow() {
            if (TargetTransition is null) {
                return;
            }
            
            EditorGUILayout.LabelField(" ");
            TargetTransition.Condition = (Condition)EditorGUILayout.ObjectField(TargetTransition.Condition, typeof(Condition), false);

            if (TargetTransition.Condition is null) {
                EditorGUILayout.LabelField("No condition!");
            } else {
                TargetTransition.Disable = EditorGUILayout.Toggle("Disable", TargetTransition.Disable);
            }
        }

        public override void DrawCurve() {
            if (EnterState is null) {
                return;
            }

            var rect = WindowRect;
            rect.y += WindowRect.height * 0.5f;
            rect.width = 1;
            rect.height = 1;
            
            DependenciesEditor.DrawNodeCurve(EnterState.WindowRect, rect, true, Color.gray);
        }
    }
}
