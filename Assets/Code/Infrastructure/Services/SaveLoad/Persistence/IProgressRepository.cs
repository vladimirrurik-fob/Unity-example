namespace Code.Infrastructure.Services.SaveLoad.Persistence
{
   // Storage boundary: the only thing that knows WHERE progress lives (a JSON file
   // on disk here). Swappable for PlayerPrefs / cloud / etc. without touching any
   // layer above.
   public interface IProgressRepository
   {
      bool HasSavedData { get; }

      string Load();

      void Save(string json);

      void Clear();
   }
}
