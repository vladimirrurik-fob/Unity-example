using System.Collections.Generic;

namespace Homework.SaveLoad
{
    // Collects every ISaveLoad so the service can fan out Save/Load over them.
    public interface ISaveLoadRegistry
    {
        IReadOnlyList<ISaveLoad> GetAll();
    }
}
