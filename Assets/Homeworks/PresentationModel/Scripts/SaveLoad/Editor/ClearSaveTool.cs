using System.IO;
using UnityEditor;
using UnityEngine;

namespace Homework.SaveLoad.EditorTools
{
    public static class ClearSaveTool
    {
        private const string FileName = "PlayerProgress.json";

        [MenuItem("Tools/SaveLoad/Clear Save File")]
        public static void Clear()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[SaveLoad] Deleted save file: {path}");
            }
            else
            {
                Debug.Log("[SaveLoad] No save file to delete.");
            }
        }
    }
}
