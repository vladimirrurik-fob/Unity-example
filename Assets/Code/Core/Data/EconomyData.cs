using System;
using System.Collections.Generic;
using Otus.Converters.Core;

namespace Code.Core.Data
{
    /// <summary>
    /// Persisted snapshot of the resource-converter economy. Lives on
    /// <see cref="PlayerProgress"/> and is (de)serialized by JsonUtility, so it uses
    /// plain public fields and lists (no dictionaries). The economy's own
    /// <c>EconomySaveLoad</c> adapter is the Information Expert for moving data between
    /// this DTO and the live services.
    /// </summary>
    [Serializable]
    public class EconomyData
    {
        public List<ResourceData> Resources = new List<ResourceData>();
        public List<ConverterData> Converters = new List<ConverterData>();
    }

    [Serializable]
    public class ResourceData
    {
        public ResourceId Id;
        public int Amount;
    }

    [Serializable]
    public class ConverterData
    {
        public ConverterId Id;
        public int Level;
    }
}
