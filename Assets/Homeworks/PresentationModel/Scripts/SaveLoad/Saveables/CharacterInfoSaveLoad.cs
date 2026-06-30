using System;
using Homework.SaveLoad.Data;
using Lessons.Architecture.PM;
using CharacterInfo = Lessons.Architecture.PM.CharacterInfo;

namespace Homework.SaveLoad
{
    // Persists the character stat values by name.
    public sealed class CharacterInfoSaveLoad : ISaveLoad
    {
        private readonly CharacterInfo _characterInfo;

        public CharacterInfoSaveLoad(CharacterInfo characterInfo)
        {
            this._characterInfo = characterInfo;
        }

        public void Save(PlayerProgress progress)
        {
            var list = progress.Player.Stats;
            list.Clear();
            foreach (var stat in this._characterInfo.GetStats())
            {
                list.Add(new StatData { Name = stat.Name, Value = stat.Value });
            }
        }

        public void Load(PlayerProgress progress)
        {
            foreach (var data in progress.Player.Stats)
            {
                try
                {
                    // Stats are seeded by GameInitializer before Load runs, so each
                    // saved name should resolve. Unknown names are skipped.
                    this._characterInfo.GetStat(data.Name).ChangeValue(data.Value);
                }
                catch (Exception)
                {
                    // Stat not present in this build — ignore.
                }
            }
        }
    }
}
