using UnityEngine;

namespace Homework.SaveLoad
{
    public sealed class SaveLoadService : ISaveLoadService
    {
        private readonly IProgressService _progress;
        private readonly ISaveLoadRegistry _registry;

        public SaveLoadService(IProgressService progress, ISaveLoadRegistry registry)
        {
            this._progress = progress;
            this._registry = registry;
        }

        public void Save()
        {
            // Progress is null until GameInitializer has loaded/seeded it. AppQuitSaver
            // can fire Save (e.g. an early OnApplicationPause) before that point — bail
            // out rather than serialize an empty/uninitialized state or NRE.
            if (this._progress.Progress == null)
            {
                Debug.LogWarning("[SaveLoad] Progress not initialized yet — skipping save.");
                return;
            }

            foreach (var saveable in this._registry.GetAll())
            {
                saveable.Save(this._progress.Progress);
            }

            this._progress.SaveProgress();
            Debug.Log("[SaveLoad] Game saved.");
        }

        public void Load()
        {
            if (this._progress.Progress == null)
            {
                return;
            }

            if (!this._progress.HasSavedProgress)
            {
                Debug.Log("[SaveLoad] No saved progress — keeping defaults.");
                return;
            }

            foreach (var saveable in this._registry.GetAll())
            {
                saveable.Load(this._progress.Progress);
            }

            Debug.Log("[SaveLoad] Game loaded.");
        }
    }
}
