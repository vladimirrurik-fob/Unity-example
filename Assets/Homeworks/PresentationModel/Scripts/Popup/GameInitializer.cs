using Lessons.Architecture.PM;
using VContainer.Unity;

namespace Homework.PresentationModel
{
    public sealed class GameInitializer : IStartable
    {
        private readonly UserInfo _userInfo;
        private readonly PlayerLevel _playerLevel;
        private readonly CharacterInfo _characterInfo;
        private readonly IPlayerPopupPresentationModel _model;
        private readonly IPopupManager _popupManager;
        private readonly PlayerPopupView _view;
        private readonly PopupVisualConfig _config;

        public GameInitializer(
            UserInfo userInfo,
            PlayerLevel playerLevel,
            CharacterInfo characterInfo,
            IPlayerPopupPresentationModel model,
            IPopupManager popupManager,
            PlayerPopupView view,
            PopupVisualConfig config)
        {
            this._userInfo = userInfo;
            this._playerLevel = playerLevel;
            this._characterInfo = characterInfo;
            this._model = model;
            this._popupManager = popupManager;
            this._view = view;
            this._config = config;
        }

        public void Start()
        {
            this._userInfo.ChangeName("Captain Rurik");
            this._userInfo.ChangeDescription("Press ADD XP to fill the bar, then LEVEL UP");
            this._userInfo.ChangeIcon(this._config.Portrait);

            this._characterInfo.AddStat(new CharacterStat("Damage", 12));
            this._characterInfo.AddStat(new CharacterStat("Speed", 7));
            this._characterInfo.AddStat(new CharacterStat("Health", 100));

            this._model.Initialize();
            this._model.CloseRequested += () => this._popupManager.Hide("PlayerPopup");

            this._popupManager.Register("PlayerPopup", this._view.gameObject);
            this._popupManager.Show("PlayerPopup");
        }
    }
}
