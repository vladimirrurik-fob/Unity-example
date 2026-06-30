using Homework.SaveLoad.Data;
using UnityEngine;

namespace Homework.SaveLoad
{
    public sealed class ProgressService : IProgressService
    {
        private readonly IProgressRepository _repository;

        public ProgressService(IProgressRepository repository)
        {
            this._repository = repository;
        }

        public PlayerProgress Progress { get; private set; }

        public bool HasSavedProgress { get; private set; }

        public void LoadProgressOrInitNew()
        {
            if (!this._repository.HasSavedData)
            {
                this.HasSavedProgress = false;
                this.Progress = new PlayerProgress();
                Debug.Log("[SaveLoad] No save file — starting fresh.");
                return;
            }

            var json = this._repository.Load();
            if (string.IsNullOrEmpty(json))
            {
                this.HasSavedProgress = false;
                this.Progress = new PlayerProgress();
                return;
            }

            this.HasSavedProgress = true;
            this.Progress = JsonUtility.FromJson<PlayerProgress>(json);
            Debug.Log("[SaveLoad] Save file loaded into memory.");
        }

        public void SaveProgress()
        {
            var json = JsonUtility.ToJson(this.Progress, prettyPrint: true);
            this._repository.Save(json);
        }
    }
}
