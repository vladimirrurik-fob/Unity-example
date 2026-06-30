using Homework.SaveLoad.Data;

namespace Homework.SaveLoad
{
    // Holds the in-memory PlayerProgress and owns its lifecycle
    // (load-from-storage on demand, persist on demand).
    public interface IProgressService
    {
        PlayerProgress Progress { get; }

        bool HasSavedProgress { get; }

        void LoadProgressOrInitNew();

        void SaveProgress();
    }
}
