using Code.Core.Abstractions;
using Code.Infrastructure.Services.SaveLoad.Progress;
using Code.Infrastructure.Services.SaveLoad.Registry;
using UnityEngine;

namespace Code.Infrastructure.Services.SaveLoad
{
   public class SaveLoadService : ISaveLoadService
   {
      private readonly IProgressService _progress;
      private readonly ISaveLoadRegistry _registry;

      public SaveLoadService(IProgressService progress,
         ISaveLoadRegistry registry)
      {
         _progress = progress;
         _registry = registry;
      }

      public void Save()
      {
         // Progress can be null before the boot system has hydrated it; bail out
         // rather than serialize an empty/uninitialized state or NRE.
         if (_progress.Progress == null)
         {
            Debug.LogWarning("[SaveLoad] Progress not initialized yet — skipping save.");
            return;
         }

         foreach (ISaveLoad saveLoad in _registry.GetSaveLoadServices())
            saveLoad.Save(_progress.Progress);

         _progress.SaveProgress();
      }

      public void Load()
      {
         if (_progress.HasLoadProgress)
         {
            foreach (ISaveLoad saveLoad in _registry.GetSaveLoadServices())
               saveLoad.Load(_progress.Progress);
         }
         else
         {
            Debug.Log("No Actual ProgressData");
         }
      }

      public void Reset()
      {
         
      }
   }
}