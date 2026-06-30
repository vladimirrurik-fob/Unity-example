using Code.Infrastructure.Services._Input;
using Code.Infrastructure.Services.SaveLoad;
using Code.Infrastructure.Services.SaveLoad.Progress;
using UnityEngine;
using VContainer.Unity;

namespace Code.GameApp.Systems
{
   /// <summary>
   /// Bridges input (Save/Load buttons) to the save/load service and runs the
   /// initial load on boot. A VContainer IInitializable entry point.
   /// </summary>
   public class SaveLoadSystem : IInitializable
   {
      private readonly ISaveLoadService _saveLoad;
      private readonly IProgressService _progress;
      private readonly IInputService _input;

      public SaveLoadSystem(ISaveLoadService saveLoad,
         IProgressService progress,
         IInputService input)
      {
         _saveLoad = saveLoad;
         _progress = progress;
         _input = input;
      }

      public void Initialize()
      {
         _input.OnSaveBtnClick += Save;
         _input.OnLoadBtnClick += Reload;
         _saveLoad.Load();
         Debug.Log("SaveLoadSystem.Initialize");
      }

      private void Save()
      {
         _saveLoad.Save();
      }

      private void Reload()
      {
         // Re-hydrate progress straight from the file, then re-apply every saveable.
         _progress.LoadProgressOrInitNew();
         _saveLoad.Load();
         Debug.Log("[SaveLoad] Reloaded from file via Load button.");
      }
   }
}
