using System;
using System.IO;
using UnityEngine;

namespace Homework.SaveLoad
{
    // Persists progress as a JSON file under Application.persistentDataPath.
    // This is the only class coupled to System.IO / the filesystem.
    public sealed class FileProgressRepository : IProgressRepository
    {
        private const string FileName = "PlayerProgress.json";

        private readonly string _filePath;

        public FileProgressRepository()
        {
            this._filePath = Path.Combine(Application.persistentDataPath, FileName);
        }

        public bool HasSavedData => File.Exists(this._filePath);

        public string Load()
        {
            try
            {
                return File.ReadAllText(this._filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoad] Failed to read {this._filePath}: {e}");
                return null;
            }
        }

        public void Save(string json)
        {
            try
            {
                File.WriteAllText(this._filePath, json);
                Debug.Log($"[SaveLoad] Progress written to {this._filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoad] Failed to write {this._filePath}: {e}");
            }
        }

        public void Clear()
        {
            try
            {
                if (File.Exists(this._filePath))
                {
                    File.Delete(this._filePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoad] Failed to clear {this._filePath}: {e}");
            }
        }
    }
}
