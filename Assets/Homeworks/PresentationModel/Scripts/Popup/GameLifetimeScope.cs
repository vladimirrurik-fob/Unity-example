using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Homework.PresentationModel
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Panel")]
        [SerializeField] private Sprite _background;
        [SerializeField] private Sprite _header;
        [SerializeField] private Sprite _sunrays;
        [SerializeField] private Sprite _portrait;
        [SerializeField] private Sprite _closeButton;

        [Header("Avatars")]
        [SerializeField] private Sprite _ava1;
        [SerializeField] private Sprite _ava2;
        [SerializeField] private Sprite _ava3;

        [Header("Stats")]
        [SerializeField] private Sprite _point;

        [Header("Progress")]
        [SerializeField] private Sprite _progressBarBackground;
        [SerializeField] private Sprite _progressBarCompleted;
        [SerializeField] private Sprite _progressBarNotCompleted;

        [Header("Buttons")]
        [SerializeField] private Sprite _buttonActive;
        [SerializeField] private Sprite _buttonInactive;

        [Header("Font")]
        [SerializeField] private Font _font;

        protected override void Configure(IContainerBuilder builder)
        {
            var config = new PopupVisualConfig
            {
                Background = this._background,
                Header = this._header,
                Sunrays = this._sunrays,
                Portrait = this._portrait,
                Ava1 = this._ava1,
                Ava2 = this._ava2,
                Ava3 = this._ava3,
                Point = this._point,
                ProgressBarBackground = this._progressBarBackground,
                ProgressBarCompleted = this._progressBarCompleted,
                ProgressBarNotCompleted = this._progressBarNotCompleted,
                ButtonActive = this._buttonActive,
                ButtonInactive = this._buttonInactive,
                CloseButton = this._closeButton,
                Font = this._font,
            };
            builder.RegisterInstance(config);

            builder.Register<Lessons.Architecture.PM.UserInfo>(Lifetime.Singleton);
            builder.Register<Lessons.Architecture.PM.PlayerLevel>(Lifetime.Singleton);
            builder.Register<Lessons.Architecture.PM.CharacterInfo>(Lifetime.Singleton);

            builder.Register<IPlayerPopupPresentationModel, PlayerPopupPresentationModel>(Lifetime.Singleton);
            builder.Register<IPopupManager, PopupManager>(Lifetime.Singleton);

            // Plain-C# view (built by the initializer) + game setup (IStartable).
            builder.Register<PlayerPopupView>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameInitializer>(Lifetime.Singleton);
        }
    }
}
