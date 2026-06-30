using System.Collections.Generic;

namespace Homework.SaveLoad
{
    // VContainer resolves IReadOnlyList<ISaveLoad> to every registered saveable,
    // so the registry is just a thin holder over that collection.
    public sealed class SaveLoadRegistry : ISaveLoadRegistry
    {
        private readonly IReadOnlyList<ISaveLoad> _saveables;

        public SaveLoadRegistry(IReadOnlyList<ISaveLoad> saveables)
        {
            this._saveables = saveables;
        }

        public IReadOnlyList<ISaveLoad> GetAll() => this._saveables;
    }
}
