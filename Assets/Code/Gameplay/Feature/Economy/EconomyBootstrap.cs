using Otus.Converters.Core;
using UnityEngine;
using VContainer.Unity;

namespace Code.Gameplay.Feature.Economy
{
    /// <summary>
    /// Entry point that brings the economy UI into the scene: builds the panel
    /// GameObject, attaches the passive view, and wires the presenter to the domain
    /// services. No scene prefab or manual reference setup is required — everything is
    /// resolved from the <c>GameLifetimeScope</c> container.
    /// </summary>
    public sealed class EconomyBootstrap : IStartable
    {
        private readonly IResourceStorage _storage;
        private readonly IConverterService _converters;
        private readonly IUpgradeManager _upgrades;

        public EconomyBootstrap(IResourceStorage storage, IConverterService converters, IUpgradeManager upgrades)
        {
            _storage = storage;
            _converters = converters;
            _upgrades = upgrades;
        }

        public void Start()
        {
            var go = new GameObject("ConvertersPanel");
            var view = go.AddComponent<ConvertersPanelView>();
            view.Initialize(_converters.ConverterIds);

            var presenter = new ConvertersPresenter(_storage, _converters, _upgrades, view);
            presenter.Start();

            // Drive the automatic converter cycles from a MonoBehaviour Update so
            // ticking is deterministic and independent of VContainer's player loop.
            var ticker = new GameObject("EconomyTicker").AddComponent<EconomyTickable>();
            ticker.Construct(_converters);
        }
    }
}
