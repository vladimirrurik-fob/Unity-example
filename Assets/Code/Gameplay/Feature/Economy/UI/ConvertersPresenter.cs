using System;
using Otus.Converters.Core;

namespace Code.Gameplay.Feature.Economy
{
    /// <summary>
    /// MVP presentation model. Mediates between the passive <see cref="ConvertersPanelView"/>
    /// and the domain services: it routes button clicks to the upgrade manager / converter
    /// service, and re-renders the whole panel whenever any domain event fires. The view
    /// and the domain never reference each other directly (Low Coupling).
    /// </summary>
    public sealed class ConvertersPresenter
    {
        private readonly IResourceStorage _storage;
        private readonly IConverterService _converters;
        private readonly IUpgradeManager _upgrades;
        private readonly ConvertersPanelView _view;

        public ConvertersPresenter(
            IResourceStorage storage,
            IConverterService converters,
            IUpgradeManager upgrades,
            ConvertersPanelView view)
        {
            _storage = storage;
            _converters = converters;
            _upgrades = upgrades;
            _view = view;
        }

        public void Start()
        {
            _view.UpgradeClicked += OnUpgradeClicked;
            _view.TapClicked += OnTapClicked;

            _storage.Changed += OnAnyChange;
            _converters.Produced += OnAnyChangeById;
            _converters.LevelChanged += OnAnyChangeById;
            _upgrades.Upgraded += OnAnyChangeById;

            Refresh();
        }

        private void OnUpgradeClicked(ConverterId id) => _upgrades.TryUpgrade(id);

        private void OnTapClicked(ConverterId id) => _converters.TryTap(id);

        private void OnAnyChange(ResourceId _, int __) => Refresh();

        private void OnAnyChangeById(ConverterId _, int __) => Refresh();

        private void OnAnyChangeById(ConverterId _) => Refresh();

        private void Refresh()
        {
            foreach (ResourceId id in Enum.GetValues(typeof(ResourceId)))
            {
                _view.SetResource(id, _storage.Get(id));
            }

            foreach (ConverterId id in _converters.ConverterIds)
            {
                _view.SetConverter(
                    id,
                    _converters.GetLevel(id),
                    _converters.GetStats(id),
                    _upgrades.GetUpgradeCost(id),
                    _upgrades.CanUpgrade(id),
                    _converters.CanTap(id));
            }
        }
    }
}
