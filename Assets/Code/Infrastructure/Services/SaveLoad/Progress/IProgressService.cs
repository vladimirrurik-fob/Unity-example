using Code.Core.Data;

namespace Code.Infrastructure.Services.SaveLoad.Progress
{
   public interface IProgressService
   {
      PlayerProgress Progress { get; }
      bool HasLoadProgress { get; }
      PlayerProgress CreateNewProgress();
      void SaveProgress();
      PlayerProgress LoadProgressOrInitNew();
   }
}