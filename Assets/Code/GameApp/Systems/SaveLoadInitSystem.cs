using Code.Infrastructure.Services.SaveLoad.Progress;
using VContainer.Unity;

namespace Code.GameApp.Systems
{
   public class SaveLoadInitSystem : IInitializable
   {
      private readonly IProgressService _progress;

      public SaveLoadInitSystem(IProgressService progress)
      {
         _progress = progress;
      }

      public void Initialize()
      {
         _progress.LoadProgressOrInitNew();
      }
   }
}