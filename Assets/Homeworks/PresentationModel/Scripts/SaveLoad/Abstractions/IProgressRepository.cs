namespace Homework.SaveLoad
{
    // Low-level storage boundary. The only thing that knows WHERE progress lives
    // (a file on disk here). Swappable for PlayerPrefs/cloud/etc. without touching
    // anything above.
    public interface IProgressRepository
    {
        bool HasSavedData { get; }

        string Load();

        void Save(string json);

        void Clear();
    }
}
