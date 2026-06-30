namespace Homework.SaveLoad
{
    // Orchestrator: asks every registered ISaveLoad to (de)serialize into the
    // shared progress, then persists the progress itself.
    public interface ISaveLoadService
    {
        void Save();

        void Load();
    }
}
