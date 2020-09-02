using UnityEditor;
using UnityEngine;

namespace Avocado.DependenciesVisualizer.Base.Editor {
    public static class SceneEditorGuiTools {
        public static void SimpleText(string text, int maxWidth = 100) {
            var labelStyle = new GUIStyle("label");
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.fontSize = 12;
            EditorGUILayout.LabelField(text, labelStyle, GUILayout.MaxWidth(maxWidth));
        }
    }
}
