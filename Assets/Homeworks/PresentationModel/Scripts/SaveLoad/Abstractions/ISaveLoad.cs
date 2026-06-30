using Homework.SaveLoad.Data;

namespace Homework.SaveLoad
{
    // Marks a piece of game state that knows how to read/write itself into the
    // shared PlayerProgress. Implemented by thin adapters over the domain.
    public interface ISaveLoad
    {
        void Save(PlayerProgress progress);

        void Load(PlayerProgress progress);
    }
}
