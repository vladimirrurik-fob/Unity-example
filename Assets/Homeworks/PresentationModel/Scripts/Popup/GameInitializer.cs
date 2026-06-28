using Lessons.Architecture.PM;
using UnityEngine;
using CharacterInfo = Lessons.Architecture.PM.CharacterInfo;
using UnityEngine.UI;
using VContainer.Unity;

namespace Homework.PresentationModel
{
    public sealed class GameInitializer : IStartable
    {
        private const string PopupKey = "PlayerPopup";

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
            this._view.Initialize();

            this._userInfo.ChangeName("Captain Rurik");
            this._userInfo.ChangeDescription("Pick an avatar, fill XP, level up");
            this._userInfo.ChangeIcon(this._config.Portrait);

            this._characterInfo.AddStat(new CharacterStat("Damage", 12));
            this._characterInfo.AddStat(new CharacterStat("Speed", 7));
            this._characterInfo.AddStat(new CharacterStat("Health", 100));

            this._model.Initialize();

            this._popupManager.Register(PopupKey, this._view.Popup);
            this._popupManager.Show(PopupKey);

            var openButton = this.CreateOpenButton(this._view.Canvas);
            openButton.SetActive(false);

            this._model.CloseRequested += () =>
            {
                this._popupManager.Hide(PopupKey);
                openButton.SetActive(true);
            };

            openButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                this._popupManager.Show(PopupKey);
                openButton.SetActive(false);
            });
        }

        private GameObject CreateOpenButton(Canvas canvas)
        {
            var go = new GameObject("OpenPopupButton", typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(300, 72);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            go.AddComponent<Button>();

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var label = labelGo.AddComponent<Text>();
            label.text = "OPEN PLAYER POPUP";
            label.alignment = TextAnchor.MiddleCenter;
            label.font = this._config.Font != null
                ? this._config.Font
                : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.color = Color.white;
            label.fontSize = 22;
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.sizeDelta = Vector2.zero;
            return go;
        }
    }
}
