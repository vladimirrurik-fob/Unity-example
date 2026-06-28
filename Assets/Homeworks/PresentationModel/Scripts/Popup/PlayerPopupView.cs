using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Homework.PresentationModel
{
    public sealed class PlayerPopupView : MonoBehaviour
    {
        private IPlayerPopupPresentationModel _model;
        private PopupVisualConfig _config;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly CompositeDisposable _statsDisposables = new CompositeDisposable();

        private Image _portrait;
        private Text _nameText;
        private Text _descriptionText;
        private Text _levelText;
        private Text _experienceText;
        private Image _progressFill;
        private Button _levelUpButton;
        private Transform _statsContainer;

        [Inject]
        public void Construct(IPlayerPopupPresentationModel model, PopupVisualConfig config)
        {
            this._model = model;
            this._config = config;

            this.BuildUI();
            this.Bind();
            this._model.StatsChanged += this.OnStatsChanged;
            this.OnStatsChanged();
        }

        private void OnDestroy()
        {
            this._disposables.Dispose();
            this._statsDisposables.Dispose();
            if (this._model != null)
            {
                this._model.StatsChanged -= this.OnStatsChanged;
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
            this._model.CanLevelUp.Subscribe(v => this._levelUpButton.interactable = v).AddTo(this._disposables);
        }

        private void OnStatsChanged()
        {
            this._statsDisposables.Clear();

            for (int i = 0; i < this._statsContainer.childCount; i++)
            {
                Destroy(this._statsContainer.GetChild(i).gameObject);
            }

            float y = 0f;
            foreach (var stat in this._model.Stats)
            {
                var text = this.CreateText(this._statsContainer, "Stat", string.Empty, 20);
                var rect = text.rectTransform;
                rect.anchoredPosition = new Vector2(0, y);
                rect.sizeDelta = new Vector2(600, 28);

                stat.Name
                    .CombineLatest(stat.Value, (name, value) => $"{name}: {value}")
                    .Subscribe(v => text.text = v)
                    .AddTo(this._statsDisposables);

                y -= 30f;
            }
        }

        private void BuildUI()
        {
            var canvasGo = new GameObject("PopupCanvas");
            canvasGo.transform.SetParent(this.transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            if (Object.FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None).Length == 0)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            var popup = this.CreateImage(canvasGo.transform, "Background", this._config.Background);
            var popupRect = popup.rectTransform;
            popupRect.sizeDelta = new Vector2(700, 680);
            popup.color = this._config.Background != null ? Color.white : new Color(0.12f, 0.13f, 0.16f, 0.95f);

            var header = this.CreateImage(popupRect, "Header", this._config.Header);
            header.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            header.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            header.rectTransform.anchoredPosition = new Vector2(0, -40);
            header.rectTransform.sizeDelta = new Vector2(700, 80);
            this._levelText = this.CreateText(header.rectTransform, "LevelText", "Level 1", 26);
            this._levelText.rectTransform.sizeDelta = new Vector2(700, 80);
            this._levelText.alignment = TextAnchor.MiddleCenter;

            this._portrait = this.CreateImage(popupRect, "Portrait", this._config.Portrait);
            this._portrait.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            this._portrait.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            this._portrait.rectTransform.anchoredPosition = new Vector2(0, -170);
            this._portrait.rectTransform.sizeDelta = new Vector2(160, 160);

            this._nameText = this.CreateText(popupRect, "NameText", string.Empty, 30);
            this._nameText.rectTransform.anchoredPosition = new Vector2(0, -290);
            this._nameText.rectTransform.sizeDelta = new Vector2(680, 40);
            this._nameText.alignment = TextAnchor.MiddleCenter;

            this._descriptionText = this.CreateText(popupRect, "DescriptionText", string.Empty, 18);
            this._descriptionText.rectTransform.anchoredPosition = new Vector2(0, -340);
            this._descriptionText.rectTransform.sizeDelta = new Vector2(680, 30);
            this._descriptionText.alignment = TextAnchor.MiddleCenter;

            var statsGo = new GameObject("Stats", typeof(RectTransform));
            statsGo.transform.SetParent(popupRect, false);
            this._statsContainer = statsGo.transform;
            ((RectTransform)this._statsContainer).anchoredPosition = new Vector2(0, -440);
            ((RectTransform)this._statsContainer).sizeDelta = new Vector2(680, 120);

            var progressBg = this.CreateImage(popupRect, "ProgressBg", null);
            progressBg.rectTransform.anchoredPosition = new Vector2(0, -560);
            progressBg.rectTransform.sizeDelta = new Vector2(600, 30);
            progressBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            this._progressFill = this.CreateImage(progressBg.rectTransform, "ProgressFill", this._config.ProgressBarFill);
            var fillRect = this._progressFill.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            this._progressFill.type = Image.Type.Filled;
            this._progressFill.fillMethod = Image.FillMethod.Horizontal;
            this._progressFill.color = this._config.ProgressBarFill != null ? Color.white : new Color(0.3f, 0.7f, 0.4f, 1f);

            this._experienceText = this.CreateText(popupRect, "ExperienceText", string.Empty, 16);
            this._experienceText.rectTransform.anchoredPosition = new Vector2(0, -600);
            this._experienceText.rectTransform.sizeDelta = new Vector2(600, 24);
            this._experienceText.alignment = TextAnchor.MiddleCenter;

            this._levelUpButton = this.CreateButton(popupRect, "LevelUpButton", "LEVEL UP");
            var levelUpRect = this._levelUpButton.GetComponent<RectTransform>();
            levelUpRect.anchoredPosition = new Vector2(110, -660);
            levelUpRect.sizeDelta = new Vector2(200, 56);
            this._levelUpButton.onClick.AddListener(() => this._model.OnLevelUpClicked());

            var addXpButton = this.CreateButton(popupRect, "AddXpButton", "ADD XP");
            var addXpRect = addXpButton.GetComponent<RectTransform>();
            addXpRect.anchoredPosition = new Vector2(-110, -660);
            addXpRect.sizeDelta = new Vector2(200, 56);
            addXpButton.onClick.AddListener(() => this._model.OnAddExperienceClicked());

            var closeButton = this.CreateButton(popupRect, "CloseButton", "X");
            var closeRect = closeButton.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1f, 1f);
            closeRect.anchorMax = new Vector2(1f, 1f);
            closeRect.anchoredPosition = new Vector2(-40, -40);
            closeRect.sizeDelta = new Vector2(50, 50);
            closeButton.onClick.AddListener(() => this._model.OnCloseClicked());
        }

        private Image CreateImage(Transform parent, string name, Sprite sprite)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = sprite != null;
            if (sprite == null)
            {
                image.color = new Color(1, 1, 1, 0);
            }

            return image;
        }

        private Text CreateText(Transform parent, string name, string content, int fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = Color.white;
            text.fontSize = fontSize;
            text.rectTransform.sizeDelta = new Vector2(400, 40);
            return text;
        }

        private Button CreateButton(Transform parent, string name, string label)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.6f, 0.9f, 1f);
            var button = go.AddComponent<Button>();

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var labelText = labelGo.AddComponent<Text>();
            labelText.text = label;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.color = Color.white;
            labelText.fontSize = 22;
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            return button;
        }
    }
}
