using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Homework.PresentationModel
{
    public interface IPlayerPopupPresentationModel : IDisposable
    {
        IReadOnlyReactiveProperty<string> Name { get; }

        IReadOnlyReactiveProperty<string> Description { get; }

        IReadOnlyReactiveProperty<Sprite> Icon { get; }

        IReadOnlyReactiveProperty<string> LevelText { get; }

        IReadOnlyReactiveProperty<string> ExperienceText { get; }

        IReadOnlyReactiveProperty<float> ExperienceProgress { get; }

        IReadOnlyReactiveProperty<bool> CanLevelUp { get; }

        IReadOnlyList<ICharacterStatPresentationModel> Stats { get; }

        event Action StatsChanged;

        event Action CloseRequested;

        void Initialize();

        void OnLevelUpClicked();

        void OnAddExperienceClicked();

        void OnCloseClicked();
    }
}
