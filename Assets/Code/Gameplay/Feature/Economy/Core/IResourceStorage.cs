using System;
using System.Collections.Generic;

namespace Otus.Converters.Core
{
    /// <summary>
    /// Information Expert for resource amounts. The sole mutator of how much of each
    /// resource the player owns; everything else (converters, upgrades, UI) reads and
    /// mutates state through this abstraction (Dependency Inversion).
    /// </summary>
    public interface IResourceStorage
    {
        int Get(ResourceId resource);
        void Add(ResourceId resource, int amount);
        void Set(ResourceId resource, int amount);
        bool CanSpend(ResourceId resource, int amount);
        bool TrySpend(ResourceId resource, int amount);

        /// <summary>Raised as (resource, newAmount) whenever an amount changes.</summary>
        event Action<ResourceId, int> Changed;
    }

    public sealed class ResourceStorage : IResourceStorage
    {
        private readonly Dictionary<ResourceId, int> _amounts = new Dictionary<ResourceId, int>();

        public event Action<ResourceId, int> Changed;

        public int Get(ResourceId resource) =>
            _amounts.TryGetValue(resource, out var amount) ? amount : 0;

        public void Add(ResourceId resource, int amount)
        {
            if (amount == 0)
            {
                return;
            }

            _amounts[resource] = Get(resource) + amount;
            Changed?.Invoke(resource, _amounts[resource]);
        }

        public void Set(ResourceId resource, int amount)
        {
            _amounts[resource] = amount;
            Changed?.Invoke(resource, amount);
        }

        public bool CanSpend(ResourceId resource, int amount) => Get(resource) >= amount;

        public bool TrySpend(ResourceId resource, int amount)
        {
            if (amount < 0 || !CanSpend(resource, amount))
            {
                return false;
            }

            _amounts[resource] = Get(resource) - amount;
            Changed?.Invoke(resource, _amounts[resource]);
            return true;
        }
    }
}
