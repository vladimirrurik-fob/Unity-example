using Code.Core.Data;

namespace Code.Core.Abstractions
{
   public interface ISaveLoad
   {
      void Save(PlayerProgress progress);
      void Load(PlayerProgress progress);
      void Reset(PlayerProgress progress);
   }
}