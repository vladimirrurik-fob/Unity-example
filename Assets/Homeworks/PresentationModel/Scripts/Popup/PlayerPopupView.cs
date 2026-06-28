using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VContainer.Unity;

namespace Homework.PresentationModel
{
    // Plain C# (no MonoBehaviour): builds the popup uGUI in code, binds to the
    // presentation model via UniRx, and exposes its root for the PopupManager.
    public sealed class PlayerPopupView : IInitializable, IDisposable
    {
        private readonly IPlayerPopupPresentationModel _model;
        private readonly PopupVisualConfig _config;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly CompositeDisposable _statsDisposables = new CompositeDisposable();

        private GameObject _popup;
        private Canvas _canvas;
        private Image _portrait;
        private Text _nameText;
        private Text _descriptionText;
        private Text _levelText;
        private Text _experienceText;
        private Image _progressFill;
        private Button _levelUpButton;
        private Image _levelUpImage;
        private RectTransform _statsContainer;

        public GameObject Popup => this._popup;
        public Canvas Canvas => this._canvas;

        public PlayerPopupView(IPlayerPopupPresentationModel model, PopupVisualConfig config)
        {
            this._model = model;
            this._config = config;
        }

        public void Initialize()
        {
            this.BuildUI();
            this.Bind();
            this._model.StatsChanged += this.OnStatsChanged;
            this.OnStatsChanged();
        }

        public void Dispose()
        {
            this._disposables.Dispose();
            this._statsDisposables.Dispose();
            if (this._model != null)
            {
                this._model.StatsChanged -= this.OnStatsChanged;
            }

            if (this._canvas != null)
            {
                UnityEngine.Object.Destroy(this._canvas.gameObject);
            }
        }

        private void Bind()
        {
            this._model.Name.Subscribe(v => this._nameText.text = v).AddTo(this._disposables);
            this._model.Description.Subscribe(v => this._descriptionText.text = v).AddTo(this._disposables);
            this._model.Icon.Subscribe(v => this._portrait.sprite = v).AddTo(this._disposables);
            this._model.LevelText.Subscribe(v => this._levelText.text = v).AddTo(this._disposables);
            this._model.ExperienceText.Subscribe(v => this._experienceText.text = v).AddTo(this._disposables);
            this._model.ExperienceProgress.Subscribe(v => this._progressFill.fillAmount = v).AddTo(this._disposables);
            this._model.CanLevelUp.Subscribe(this.OnCanLevelUpChanged).AddTo(this._disposables);
        }

        private void OnCanLevelUpChanged(bool canLevelUp)
        {
            this._levelUpButton.interactable = canLevelUp;
            if (this._levelUpImage != null && this._config.ButtonActive != null && this._config.ButtonInactive != null)
            {
                this._levelUpImage.sprite = canLevelUp ? this._config.ButtonActive : this._config.ButtonInactive;
            }

            if (this._config.ProgressBarCompleted != null && this._config.ProgressBarNotCompleted != null)
            {
                this._progressFill.sprite = canLevelUp
                    ? this._config.ProgressBarCompleted
                    : this._config.ProgressBarNotCompleted;
            }
        }

        private void OnStatsChanged()
        {
            this._statsDisposables.Clear();
            for (int i = 0; i < this._statsContainer.childCount; i++)
            {
                UnityEngine.Object.Destroy(this._statsContainer.GetChild(i).gameObject);
            }

            float y = 0f;
            foreach (var stat in this._model.Stats)
            {
                var row = this.Make(this._statsContainer, "Stat", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0, y), new Vector2(640, 30));
                var point = this.CreateImage(this.Make(row, "Point", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-280, 0), new Vector2(26, 26)), this._config.Point);
                point.preserveAspect = true;
                var value = this.CreateText(this.Make(row, "Value", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, 0), new Vector2(560, 30)), string.Empty, 22);

                stat.Name.CombineLatest(stat.Value, (n, v) => $"{n}: {v}")
                    .Subscribe(v => value.text = v)
                    .AddTo(this._statsDisposables);

                y -= 34f;
            }
        }

        private void BuildUI()
        {
            var canvasGo = new GameObject("PopupCanvas");
            this._canvas = canvasGo.AddComponent<Canvas>();
            this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(760, 1100);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;
            canvasGo.AddComponent<GraphicRaycaster>();

            if (UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length == 0)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            // Root popup (centered). This is what the PopupManager shows/hides.
            this._popup = new GameObject("Popup", typeof(RectTransform));
            this._popup.transform.SetParent(canvasGo.transform, false);
            var popupRect = this._popup.GetComponent<RectTransform>();
            this.Set(popupRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760, 1100));
            var popupImage = this._popup.AddComponent<Image>();
            popupImage.sprite = this._config.Background;
            popupImage.type = this._config.Background != null ? Image.Type.Sliced : Image.Type.Simple;
            popupImage.color = this._config.Background != null ? Color.white : new Color(0.12f, 0.13f, 0.16f, 0.97f);

            this.CreateImage(this.Top(popupRect, "Header", new Vector2(0, -70), new Vector2(760, 130)), this._config.Header);
            this._levelText = this.CreateText(this.Top(popupRect, "LevelText", new Vector2(0, -70), new Vector2(700, 80)), "Level 1", 30);
            this._levelText.alignment = TextAnchor.MiddleCenter;

            this.CreateImage(this.Top(popupRect, "Sunrays", new Vector2(0, -280), new Vector2(400, 400)), this._config.Sunrays);
            this._portrait = this.CreateImage(this.Top(popupRect, "Portrait", new Vector2(0, -280), new Vector2(230, 230)), this._config.Portrait);

            this.AvatarButton(this.Top(popupRect, "Ava1", new Vector2(-150, -450), new Vector2(86, 86)), this._config.Ava1);
            this.AvatarButton(this.Top(popupRect, "Ava2", new Vector2(0, -450), new Vector2(86, 86)), this._config.Ava2);
            this.AvatarButton(this.Top(popupRect, "Ava3", new Vector2(150, -450), new Vector2(86, 86)), this._config.Ava3);

            this._nameText = this.CreateText(this.Top(popupRect, "NameText", new Vector2(0, -555), new Vector2(700, 48)), string.Empty, 30);
            this._nameText.alignment = TextAnchor.MiddleCenter;
            this._descriptionText = this.CreateText(this.Top(popupRect, "DescriptionText", new Vector2(0, -608), new Vector2(700, 32)), string.Empty, 19);
            this._descriptionText.alignment = TextAnchor.MiddleCenter;

            var statsGo = new GameObject("Stats", typeof(RectTransform));
            statsGo.transform.SetParent(popupRect, false);
            this._statsContainer = statsGo.GetComponent<RectTransform>();
            this.Set(this._statsContainer, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -660), Vector2.zero);

            var barBgRect = this.Top(popupRect, "ProgressBarBg", new Vector2(0, -870), new Vector2(620, 40));
            var barBg = this.CreateImage(barBgRect, this._config.ProgressBarBackground);
            barBg.color = this._config.ProgressBarBackground != null ? Color.white : new Color(0.18f, 0.18f, 0.2f, 1f);
            barBg.type = this._config.ProgressBarBackground != null ? Image.Type.Sliced : Image.Type.Simple;

            var fillRect = this.Make(barBgRect, "Fill", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            this._progressFill = this.CreateImage(fillRect, this._config.ProgressBarNotCompleted);
            this._progressFill.type = Image.Type.Filled;
            this._progressFill.fillMethod = Image.FillMethod.Horizontal;
            this._progressFill.color = this._config.ProgressBarNotCompleted != null ? Color.white : new Color(0.3f, 0.7f, 0.4f, 1f);

            this._experienceText = this.CreateText(this.Top(popupRect, "ExperienceText", new Vector2(0, -918), new Vector2(620, 28)), string.Empty, 18);
            this._experienceText.alignment = TextAnchor.MiddleCenter;

            var levelUpRect = this.Top(popupRect, "LevelUpButton", new Vector2(130, -995), new Vector2(230, 66));
            this._levelUpButton = this.CreateButton(levelUpRect, "LEVEL UP", this._config.ButtonActive, () => this._model.OnLevelUpClicked());
            this._levelUpImage = levelUpRect.GetComponent<Image>();

            var addXpRect = this.Top(popupRect, "AddXpButton", new Vector2(-130, -995), new Vector2(230, 66));
            this.CreateButton(addXpRect, "ADD XP", this._config.ButtonActive, () => this._model.OnAddExperienceClicked());

            var closeRect = this.Make(popupRect, "CloseButton", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(-55, -55), new Vector2(64, 64));
            this.CreateButton(closeRect, "X", this._config.CloseButton, () => this._model.OnCloseClicked());
        }

        private void AvatarButton(RectTransform parent, Sprite ava)
        {
            var image = this.CreateImage(parent, ava);
            image.preserveAspect = true;
            var button = parent.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => this._model.OnIconSelected(ava));
        }

        private RectTransform Top(RectTransform parent, string name, Vector2 pos, Vector2 size)
        {
            return this.Make(parent, name, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), pos, size);
        }

        private RectTransform Make(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            this.Set(rt, anchorMin, anchorMax, pivot, pos, size);
            return rt;
        }

        private void Set(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        private Image CreateImage(RectTransform parent, Sprite sprite)
        {
            var image = parent.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = sprite != null;
            image.type = Image.Type.Simple;
            image.color = sprite != null ? Color.white : new Color(1, 1, 1, 0);
            return image;
        }

        private Text CreateText(RectTransform parent, string content, int fontSize)
        {
            var text = parent.gameObject.AddComponent<Text>();
            text.text = content;
            text.font = this._config.Font != null
                ? this._config.Font
                : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = new Color(0.10f, 0.11f, 0.16f);
            text.fontSize = fontSize;
            text.raycastTarget = false;
            return text;
        }

        private Button CreateButton(RectTransform parent, string label, Sprite sprite, UnityEngine.Events.UnityAction onClick)
        {
            var image = parent.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = sprite != null ? Color.white : new Color(0.3f, 0.6f, 0.9f, 1f);
            image.type = sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            var button = parent.gameObject.AddComponent<Button>();
            button.onClick.AddListener(onClick);

            var labelRect = this.Make(parent, "Label", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var labelText = this.CreateText(labelRect, label, 22);
            labelText.alignment = TextAnchor.MiddleCenter;
            return button;
        }
    }
}
