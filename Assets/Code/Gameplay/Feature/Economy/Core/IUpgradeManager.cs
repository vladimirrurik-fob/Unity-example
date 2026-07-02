using System;

namespace Otus.Converters.Core
{
    /// <summary>
    /// Controller for the upgrade transaction. It is the single place that knows an
    /// upgrade is paid for in Gold and applies the level change — the Information Expert
    /// for "what does it cost and mean to upgrade". Domain rules (max level) are
    /// delegated to <see cref="IConverterService"/>; amounts to
    /// <see cref="IResourceStorage"/>; the UI never touches either directly.
    /// </summary>
    public interface IUpgradeManager
    {
        bool CanUpgrade(ConverterId id);
        bool TryUpgrade(ConverterId id);
        int GetUpgradeCost(ConverterId id);

        /// <summary>Raised as (id, newLevel) after a successful upgrade.</summary>
        event Action<ConverterId, int> Upgraded;
    }

    public sealed class UpgradeManager : IUpgradeManager
    {
        private readonly IConverterService _converters;
        private readonly IResourceStorage _storage;

        public UpgradeManager(IConverterService converters, IResourceStorage storage)
        {
            _converters = converters;
            _storage = storage;
        }

        public event Action<ConverterId, int> Upgraded;

        public int GetUpgradeCost(ConverterId id) => _converters.GetUpgradeCost(id);

        public bool CanUpgrade(ConverterId id)
        {
            if (!_converters.CanLevelUp(id))
            {
                return false;
            }

            return _storage.CanSpend(ResourceId.Gold, _converters.GetUpgradeCost(id));
        }

        public bool TryUpgrade(ConverterId id)
        {
            if (!CanUpgrade(id))
            {
                return false;
            }

            int cost = _converters.GetUpgradeCost(id);
            if (!_storage.TrySpend(ResourceId.Gold, cost))
            {
                return false;
            }

            _converters.CommitLevelUp(id);
            int newLevel = _converters.GetLevel(id);
            Upgraded?.Invoke(id, newLevel);
            return true;
        }
    }
}
