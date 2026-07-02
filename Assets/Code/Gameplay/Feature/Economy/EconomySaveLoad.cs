using System;
using Code.Core.Abstractions;
using Code.Core.Data;
using Otus.Converters.Core;

namespace Code.Gameplay.Feature.Economy
{
    /// <summary>
    /// <see cref="ISaveLoad"/> adapter for the economy: the Information Expert for how
    /// resource amounts and converter levels map to/from the persisted
    /// <see cref="EconomyData"/> DTO. Registered as ISaveLoad so the existing
    /// <c>SaveLoadRegistry</c> collects it automatically (same pattern as
    /// <c>PlayerPositionSaveLoad</c>) — no changes to the save/load stack itself.
    /// </summary>
    public sealed class EconomySaveLoad : ISaveLoad
    {
        private readonly IResourceStorage _storage;
        private readonly IConverterService _converters;

        public EconomySaveLoad(IResourceStorage storage, IConverterService converters)
        {
            _storage = storage;
            _converters = converters;
        }

        public void Save(PlayerProgress progress)
        {
            var data = progress.EconomyData ?? (progress.EconomyData = new EconomyData());

            data.Resources.Clear();
            foreach (ResourceId id in Enum.GetValues(typeof(ResourceId)))
            {
                data.Resources.Add(new ResourceData { Id = id, Amount = _storage.Get(id) });
            }

            data.Converters.Clear();
            foreach (ConverterId id in _converters.ConverterIds)
            {
                data.Converters.Add(new ConverterData { Id = id, Level = _converters.GetLevel(id) });
            }
        }

        public void Load(PlayerProgress progress)
        {
            var data = progress.EconomyData;
            if (data == null)
            {
                return;
            }

            foreach (ResourceData resource in data.Resources)
            {
                _storage.Set(resource.Id, resource.Amount);
            }

            foreach (ConverterData converter in data.Converters)
            {
                _converters.RestoreLevel(converter.Id, converter.Level);
            }
        }

        public void Reset(PlayerProgress progress)
        {
            foreach (ResourceId id in Enum.GetValues(typeof(ResourceId)))
            {
                _storage.Set(id, 0);
            }

            foreach (ConverterId id in _converters.ConverterIds)
            {
                _converters.RestoreLevel(id, 1);
            }
        }
    }
}
