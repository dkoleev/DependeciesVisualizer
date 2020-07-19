using System.IO;
using DependenciesVisualizer.Base.Editor.Scripts.State;
using UnityEditor;
using UnityEngine;

namespace DependenciesVisualizer.Base.Editor.Scripts {
    public static class StateManager {
        public static void SaveState(MainState state) {
            string path = null;
            path = "Assets/Resources/GameJSONData/ItemInfo.json";
  
            string str = state.ToString();
            using (FileStream fs = new FileStream(path, FileMode.Create)){
                using (StreamWriter writer = new StreamWriter(fs)){
                    writer.Write(str);
                }
            }
            AssetDatabase.Refresh ();
         //   var potion = JsonUtility.ToJson(State);
         //   System.IO.File.WriteAllText(Application.persistentDataPath + "/PotionData.json", potion);
        }

        public static void LoadState() {
            
        }
    }
}
