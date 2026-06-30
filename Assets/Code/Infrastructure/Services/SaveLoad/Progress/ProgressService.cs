using Code.Core.Data;
using Code.Infrastructure.Services.SaveLoad.Persistence;
using UnityEngine;

namespace Code.Infrastructure.Services.SaveLoad.Progress
{
   // Holds the in-memory PlayerProgress and owns its lifecycle: hydrate from the
   // file repository on demand, persist on demand. No PlayerPrefs — file-backed.
   public class ProgressService : IProgressService
   {
      private readonly IProgressRepository _repository;

      public ProgressService(IProgressRepository repository)
      {
         _repository = repository;
      }

      public PlayerProgress Progress { get; private set; }

      public bool HasLoadProgress { get; private set; }

      public PlayerProgress CreateNewProgress()
      {
         HasLoadProgress = false;
         return Progress = new PlayerProgress();
      }

      public void SaveProgress()
      {
         if (Progress == null)
         {
            return;
         }

         var json = JsonUtility.ToJson(Progress, true);
         _repository.Save(json);
      }

      public PlayerProgress LoadProgressOrInitNew()
      {
         if (!_repository.HasSavedData)
         {
            Debug.Log("[SaveLoad] No progress data found — starting fresh.");
            return CreateNewProgress();
         }

         var json = _repository.Load();
         if (string.IsNullOrEmpty(json))
         {
            return CreateNewProgress();
         }

         HasLoadProgress = true;
         Progress = JsonUtility.FromJson<PlayerProgress>(json);
         Debug.Log("[SaveLoad] Progress loaded from file.");
         return Progress;
      }
   }
}
