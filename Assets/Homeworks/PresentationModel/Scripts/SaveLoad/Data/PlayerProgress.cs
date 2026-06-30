using System;
using System.Collections.Generic;

namespace Homework.SaveLoad.Data
{
    // Composite save DTO. Everything we persist lives here. JsonUtility-friendly:
    // only public fields of [Serializable] types, no dictionaries.
    [Serializable]
    public sealed class PlayerProgress
    {
        public PlayerData Player = new PlayerData();
    }

    [Serializable]
    public sealed class PlayerData
    {
        public int Level = 1;
        public int Experience;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public List<StatData> Stats = new List<StatData>();
    }

    [Serializable]
    public sealed class StatData
    {
        public string Name;
        public int Value;
    }
}
