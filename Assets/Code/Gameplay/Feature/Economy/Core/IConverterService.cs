using System;
using System.Collections.Generic;

namespace Otus.Converters.Core
{
    /// <summary>
    /// Pure-fabrication controller over the set of converters: owns the runtime
    /// instances, drives their automatic cycles (<see cref="Tick"/>), and exposes the
    /// read/state operations the UI and <see cref="IUpgradeManager"/> need. Has no
    /// knowledge of the upgrade currency (Gold) — that rule lives in the upgrade
    /// manager, keeping responsibilities cohesive.
    /// </summary>
    public interface IConverterService
    {
        IReadOnlyList<ConverterId> ConverterIds { get; }

        int GetLevel(ConverterId id);
        ConverterStats GetStats(ConverterId id);
        int GetUpgradeCost(ConverterId id);

        /// <summary>True when the converter has not reached its max level.</summary>
        bool CanLevelUp(ConverterId id);

        /// <summary>Advances the level by one and raises <see cref="LevelChanged"/>.</summary>
        void CommitLevelUp(ConverterId id);

        bool CanTap(ConverterId id);
        bool TryTap(ConverterId id);

        /// <summary>Restore a level previously persisted (clamped to [1, MaxLevel]).</summary>
        void RestoreLevel(ConverterId id, int level);

        /// <summary>Advance all automatic cycles by <paramref name="dt"/> seconds.</summary>
        void Tick(float dt);

        /// <summary>Raised whenever a converter produces output (auto cycle or tap).</summary>
        event Action<ConverterId> Produced;

        /// <summary>Raised as (id, newLevel) after a level change.</summary>
        event Action<ConverterId, int> LevelChanged;
    }

    public sealed class ResourceConverterService : IConverterService
    {
        private readonly IResourceStorage _storage;
        private readonly IReadOnlyDictionary<ConverterId, ConverterConfig> _configs;
        private readonly Dictionary<ConverterId, Converter> _converters;

        public ResourceConverterService(
            IResourceStorage storage,
            IReadOnlyDictionary<ConverterId, ConverterConfig> configs)
        {
            _storage = storage;
            _configs = configs;
            _converters = new Dictionary<ConverterId, Converter>();

            var ids = new List<ConverterId>();
            foreach (var pair in configs)
            {
                _converters[pair.Key] = new Converter(pair.Key, level: 1);
                ids.Add(pair.Key);
            }

            ConverterIds = ids;
        }

        public IReadOnlyList<ConverterId> ConverterIds { get; }

        public event Action<ConverterId> Produced;
        public event Action<ConverterId, int> LevelChanged;

        public void Tick(float dt)
        {
            foreach (var pair in _converters)
            {
                if (pair.Value.Tick(dt, _configs[pair.Key], _storage))
                {
                    Produced?.Invoke(pair.Key);
                }
            }
        }

        public int GetLevel(ConverterId id) => _converters[id].Level;

        public ConverterStats GetStats(ConverterId id) =>
            _configs[id].GetStats(_converters[id].Level);

        public int GetUpgradeCost(ConverterId id) =>
            _configs[id].GetUpgradeCost(_converters[id].Level);

        public bool CanLevelUp(ConverterId id) => _converters[id].Level < _configs[id].MaxLevel;

        public void CommitLevelUp(ConverterId id)
        {
            var converter = _converters[id];
            if (converter.Level >= _configs[id].MaxLevel)
            {
                return;
            }

            int newLevel = converter.Level + 1;
            converter.SetLevel(newLevel);
            LevelChanged?.Invoke(id, newLevel);
        }

        public void RestoreLevel(ConverterId id, int level)
        {
            int clamped = Math.Max(1, Math.Min(_configs[id].MaxLevel, level));
            _converters[id].SetLevel(clamped);
        }

        public bool CanTap(ConverterId id) => _converters[id].CanTap;

        public bool TryTap(ConverterId id)
        {
            if (!_converters[id].TryTap(_configs[id], _storage))
            {
                return false;
            }

            Produced?.Invoke(id);
            return true;
        }
    }
}
