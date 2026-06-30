using System.Collections.Generic;
using Homework.SaveLoad;
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
        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;

        private readonly Dictionary<CharacterStat, int> _upgrades = new Dictionary<CharacterStat, int>();

        public GameInitializer(
            UserInfo userInfo,
            PlayerLevel playerLevel,
            CharacterInfo characterInfo,
            IPlayerPopupPresentationModel model,
            IPopupManager popupManager,
            PlayerPopupView view,
            PopupVisualConfig config,
            IProgressService progressService,
            ISaveLoadService saveLoadService)
        {
            this._userInfo = userInfo;
            this._playerLevel = playerLevel;
            this._characterInfo = characterInfo;
            this._model = model;
            this._popupManager = popupManager;
            this._view = view;
            this._config = config;
            this._progressService = progressService;
            this._saveLoadService = saveLoadService;
        }

        public void Start()
        {
            this._view.Initialize();

            this._userInfo.ChangeName("Captain Rurik");
            this._userInfo.ChangeDescription("Pick an avatar, fill XP, level up");
            this._userInfo.ChangeIcon(this._config.Portrait);

            this.AddStat("Damage", 12, 5);
            this.AddStat("Speed", 7, 2);
            this.AddStat("Health", 100, 20);

            this._playerLevel.OnLevelUp += this.OnLevelUp;

            this._model.Initialize();

            this._popupManager.Register(PopupKey, this._view.Popup);
            this._popupManager.Show(PopupKey);

            // Load saved progress and overlay it on top of the seeded defaults.
            // (Seeding runs first so stats exist for CharacterInfoSaveLoad to fill.)
            this._progressService.LoadProgressOrInitNew();
            this._saveLoadService.Load();

            this.CreatePersistenceButtons(this._view.Canvas);

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

        private void AddStat(string name, int baseValue, int perLevelIncrement)
        {
            var stat = new CharacterStat(name, baseValue);
            this._characterInfo.AddStat(stat);
            this._upgrades[stat] = perLevelIncrement;
        }

        private void OnLevelUp()
        {
            foreach (var pair in this._upgrades)
            {
                pair.Key.ChangeValue(pair.Key.Value + pair.Value);
            }
        }

        private void CreatePersistenceButtons(Canvas canvas)
        {
            var save = this.CreateTextButton(canvas, "SaveButton", "SAVE", new Vector2(-90, -540), () => this._saveLoadService.Save());
            var load = this.CreateTextButton(canvas, "LoadButton", "LOAD", new Vector2(90, -540), () =>
            {
                this._progressService.LoadProgressOrInitNew();
                this._saveLoadService.Load();
            });
        }

        private GameObject CreateTextButton(Canvas canvas, string name, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(canvas.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = new Vector2(160, 56);

            var image = go.AddComponent<Image>();
            image.sprite = this._config.ButtonActive;
            image.type = this._config.ButtonActive != null ? Image.Type.Sliced : Image.Type.Simple;
            image.color = this._config.ButtonActive != null ? Color.white : new Color(0.3f, 0.6f, 0.9f, 1f);

            var button = go.AddComponent<Button>();
            button.onClick.AddListener(onClick);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var text = labelGo.AddComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = this._config.Font != null
                ? this._config.Font
                : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = Color.white;
            text.fontSize = 22;
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.sizeDelta = Vector2.zero;
            return go;
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
