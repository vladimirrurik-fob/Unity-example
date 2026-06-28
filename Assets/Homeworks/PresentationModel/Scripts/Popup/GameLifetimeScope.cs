using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Homework.PresentationModel
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private Sprite _background;
        [SerializeField] private Sprite _header;
        [SerializeField] private Sprite _portrait;
        [SerializeField] private Sprite _progressFill;
        [SerializeField] private Sprite _buttonActive;
        [SerializeField] private Sprite _buttonInactive;
        [SerializeField] private Sprite _closeButton;
        [SerializeField] private PlayerPopupView _popupView;

        protected override void Configure(IContainerBuilder builder)
        {
            var config = new PopupVisualConfig
            {
                Background = this._background,
                Header = this._header,
                Portrait = this._portrait,
                ProgressBarFill = this._progressFill,
                ButtonActive = this._buttonActive,
                ButtonInactive = this._buttonInactive,
                CloseButton = this._closeButton,
            };
            builder.RegisterInstance(config);

            builder.Register<Lessons.Architecture.PM.UserInfo>(Lifetime.Singleton);
            builder.Register<Lessons.Architecture.PM.PlayerLevel>(Lifetime.Singleton);
            builder.Register<Lessons.Architecture.PM.CharacterInfo>(Lifetime.Singleton);

            builder.Register<IPlayerPopupPresentationModel, PlayerPopupPresentationModel>(Lifetime.Singleton);
            builder.Register<IPopupManager, PopupManager>(Lifetime.Singleton);

            if (this._popupView != null)
            {
                builder.RegisterComponent(this._popupView).AsSelf();
            }

            builder.RegisterEntryPoint<GameInitializer>(Lifetime.Singleton);
        }
    }
}
