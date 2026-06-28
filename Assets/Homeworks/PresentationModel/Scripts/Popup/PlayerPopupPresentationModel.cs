using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Lessons.Architecture.PM;
using CharacterInfo = Lessons.Architecture.PM.CharacterInfo;

namespace Homework.PresentationModel
{
    public sealed class PlayerPopupPresentationModel : IPlayerPopupPresentationModel
    {
        public IReadOnlyReactiveProperty<string> Name => this._name;
        public IReadOnlyReactiveProperty<string> Description => this._description;
        public IReadOnlyReactiveProperty<Sprite> Icon => this._icon;
        public IReadOnlyReactiveProperty<string> LevelText => this._levelText;
        public IReadOnlyReactiveProperty<string> ExperienceText => this._experienceText;
        public IReadOnlyReactiveProperty<float> ExperienceProgress => this._progress;
        public IReadOnlyReactiveProperty<bool> CanLevelUp => this._canLevelUp;
        public IReadOnlyList<ICharacterStatPresentationModel> Stats => this._stats;

        public event Action StatsChanged;
        public event Action CloseRequested;

        private readonly ReactiveProperty<string> _name = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _description = new ReactiveProperty<string>();
        private readonly ReactiveProperty<Sprite> _icon = new ReactiveProperty<Sprite>();
        private readonly ReactiveProperty<string> _levelText = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _experienceText = new ReactiveProperty<string>();
        private readonly ReactiveProperty<float> _progress = new ReactiveProperty<float>();
        private readonly ReactiveProperty<bool> _canLevelUp = new ReactiveProperty<bool>();

        private readonly UserInfo _userInfo;
        private readonly PlayerLevel _playerLevel;
        private readonly CharacterInfo _characterInfo;

        private readonly List<ICharacterStatPresentationModel> _stats = new List<ICharacterStatPresentationModel>();

        public PlayerPopupPresentationModel(UserInfo userInfo, PlayerLevel playerLevel, CharacterInfo characterInfo)
        {
            this._userInfo = userInfo;
            this._playerLevel = playerLevel;
            this._characterInfo = characterInfo;
        }

        public void Initialize()
        {
            this._userInfo.OnNameChanged += this.OnNameChanged;
            this._userInfo.OnDescriptionChanged += this.OnDescriptionChanged;
            this._userInfo.OnIconChanged += this.OnIconChanged;
            this._playerLevel.OnExperienceChanged += this.OnExperienceChanged;
            this._playerLevel.OnLevelUp += this.OnLevelUp;
            this._characterInfo.OnStatAdded += this.OnStatAdded;
            this._characterInfo.OnStatRemoved += this.OnStatRemoved;

            this.RefreshAll();
            this.RebuildStats();
        }

        public void OnLevelUpClicked() => this._playerLevel.LevelUp();

        public void OnAddExperienceClicked() => this._playerLevel.AddExperience(25);

        public void OnIconSelected(Sprite icon) => this._userInfo.ChangeIcon(icon);

        public void OnCloseClicked() => this.CloseRequested?.Invoke();

        public void Dispose()
        {
            this._userInfo.OnNameChanged -= this.OnNameChanged;
            this._userInfo.OnDescriptionChanged -= this.OnDescriptionChanged;
            this._userInfo.OnIconChanged -= this.OnIconChanged;
            this._playerLevel.OnExperienceChanged -= this.OnExperienceChanged;
            this._playerLevel.OnLevelUp -= this.OnLevelUp;
            this._characterInfo.OnStatAdded -= this.OnStatAdded;
            this._characterInfo.OnStatRemoved -= this.OnStatRemoved;

            foreach (var stat in this._stats)
            {
                stat.Dispose();
            }

            this._stats.Clear();
        }

        private void OnNameChanged(string _) => this._name.Value = this._userInfo.Name ?? string.Empty;
        private void OnDescriptionChanged(string _) => this._description.Value = this._userInfo.Description ?? string.Empty;
        private void OnIconChanged(Sprite _) => this._icon.Value = this._userInfo.Icon;
        private void OnExperienceChanged(int _) => this.RefreshExperience();
        private void OnLevelUp() => this.RefreshLevel();

        private void RefreshAll()
        {
            this._name.Value = this._userInfo.Name ?? string.Empty;
            this._description.Value = this._userInfo.Description ?? string.Empty;
            this._icon.Value = this._userInfo.Icon;
            this.RefreshLevel();
        }

        private void RefreshLevel()
        {
            this._levelText.Value = $"Level {this._playerLevel.CurrentLevel}";
            this.RefreshExperience();
        }

        private void RefreshExperience()
        {
            int required = this._playerLevel.RequiredExperience;
            int current = this._playerLevel.CurrentExperience;
            this._experienceText.Value = $"{current} / {required}";
            this._progress.Value = required > 0 ? (float)current / required : 0f;
            this._canLevelUp.Value = this._playerLevel.CanLevelUp();
        }

        private void OnStatAdded(CharacterStat stat)
        {
            this._stats.Add(new CharacterStatPresentationModel(stat));
            this.StatsChanged?.Invoke();
        }

        private void OnStatRemoved(CharacterStat stat) => this.RebuildStats();

        private void RebuildStats()
        {
            foreach (var stat in this._stats)
            {
                stat.Dispose();
            }

            this._stats.Clear();
            foreach (var stat in this._characterInfo.GetStats())
            {
                this._stats.Add(new CharacterStatPresentationModel(stat));
            }

            this.StatsChanged?.Invoke();
        }
    }
}
