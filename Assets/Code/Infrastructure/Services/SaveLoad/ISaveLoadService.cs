namespace Code.Infrastructure.Services.SaveLoad
{
   public interface ISaveLoadService
   {
      void Save();
      void Load();
      void Reset();
   }
}