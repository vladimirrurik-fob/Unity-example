using System;
using System.IO;
using UnityEngine;

namespace Code.Infrastructure.Services.SaveLoad.Persistence
{
   // Persists progress as JSON under Application.persistentDataPath.
   // The single System.IO touchpoint in the whole subsystem.
   public sealed class FileProgressRepository : IProgressRepository
   {
      private const string FileName = "PlayerProgress.json";

      private readonly string _filePath;

      public FileProgressRepository()
      {
         _filePath = Path.Combine(Application.persistentDataPath, FileName);
      }

      public bool HasSavedData => File.Exists(_filePath);

      public string Load()
      {
         try
         {
            return File.ReadAllText(_filePath);
         }
         catch (Exception e)
         {
            Debug.LogError($"[SaveLoad] Failed to read {_filePath}: {e}");
            return null;
         }
      }

      public void Save(string json)
      {
         try
         {
            File.WriteAllText(_filePath, json);
            Debug.Log($"[SaveLoad] Progress written to {_filePath}");
         }
         catch (Exception e)
         {
            Debug.LogError($"[SaveLoad] Failed to write {_filePath}: {e}");
         }
      }

      public void Clear()
      {
         try
         {
            if (File.Exists(_filePath))
            {
               File.Delete(_filePath);
            }
         }
         catch (Exception e)
         {
            Debug.LogError($"[SaveLoad] Failed to clear {_filePath}: {e}");
         }
      }
   }
}
