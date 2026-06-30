using UnityEditor;
using UnityEngine;

namespace Code.Editor.Tools
{
   public class ClearSavePrefsTool
   {
      [MenuItem("Tools/Clear Save Prefs")]
      public static void ClearPrefs()
      {
         PlayerPrefs.DeleteAll();
         PlayerPrefs.Save();
      }
   }
}