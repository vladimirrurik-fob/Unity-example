using System.Collections.Generic;
using Otus.Converters.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Feature.Economy
{
    /// <summary>
    /// Passive uGUI view (MVP). It owns no domain state — it only renders what the
    /// presenter pushes and forwards button clicks as plain events. The whole panel is
    /// built procedurally in <see cref="Initialize"/> so the scene needs no manual wiring
    /// beyond the <c>EconomyBootstrap</c> entry point.
    /// </summary>
    public sealed class ConvertersPanelView : MonoBehaviour
    {
        public event System.Action<ConverterId> UpgradeClicked;
        public event System.Action<ConverterId> TapClicked;

        private readonly Dictionary<ResourceId, Text> _resourceLabels = new Dictionary<ResourceId, Text>();
        private readonly Dictionary<ConverterId, Card> _cards = new Dictionary<ConverterId, Card>();
        private Font _font;

        public void Initialize(IReadOnlyList<ConverterId> ids)
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            BuildRoot();
            BuildResourceBar();
            foreach (ConverterId id in ids)
            {
                BuildCard(id);
            }
        }

        public void SetResource(ResourceId id, int amount)
        {
            if (_resourceLabels.TryGetValue(id, out Text label))
            {
                label.text = $"{id}: {amount}";
            }
        }

        public void SetConverter(
            ConverterId id,
            int level,
            ConverterStats stats,
            int upgradeCost,
            bool canUpgrade,
            bool canTap)
        {
            if (!_cards.TryGetValue(id, out Card card))
            {
                return;
            }

            card.LevelLabel.text = $"{id}  -  Lv {level}";
            string input = stats.HasInput ? $"{stats.InputAmount} {stats.InputResource} -> " : string.Empty;
            card.StatsLabel.text =
                $"{input}{stats.OutputAmount} {stats.OutputResource}\n" +
                $"cycle {stats.CycleSeconds:0.0}s   tap +{stats.TapBonusAmount}";

            bool maxed = level >= 5;
            card.UpgradeLabel.text = maxed ? "MAX" : $"Upgrade  ({upgradeCost} Gold)";
            card.UpgradeButton.interactable = canUpgrade && !maxed;
            card.TapButton.interactable = canTap;
        }

        private void BuildRoot()
        {
            var canvasGo = new GameObject("EconomyCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Semi-transparent backing panel anchored top-left.
            var panelGo = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelGo.transform.SetParent(canvasGo.transform, false);
            panelGo.GetComponent<Image>().color = new Color(0.05f, 0.06f, 0.09f, 0.75f);
            var prt = (RectTransform)panelGo.transform;
            prt.anchorMin = new Vector2(0f, 1f);
            prt.anchorMax = new Vector2(0f, 1f);
            prt.pivot = new Vector2(0f, 1f);
            prt.anchoredPosition = new Vector2(16f, -16f);
            prt.sizeDelta = new Vector2(540f, 520f);

            var vlg = panelGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8f;
            vlg.padding = new RectOffset(16, 16, 16, 16);
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childAlignment = TextAnchor.UpperCenter;
        }

        private RectTransform Panel() =>
            (RectTransform)transform.GetChild(0).GetChild(0); // EconomyCanvas -> Panel

        private void BuildResourceBar()
        {
            var row = NewRow(Panel(), "Resources");
            foreach (ResourceId id in System.Enum.GetValues(typeof(ResourceId)))
            {
                var label = NewText(row, id.ToString(), 26, TextAnchor.MiddleLeft);
                _resourceLabels[id] = label.GetComponent<Text>();
            }
        }

        private void BuildCard(ConverterId id)
        {
            var cardGo = new GameObject(id.ToString(), typeof(RectTransform), typeof(Image));
            cardGo.transform.SetParent(Panel(), false);
            cardGo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.08f);
            var crt = (RectTransform)cardGo.transform;
            crt.sizeDelta = new Vector2(0f, 120f);

            var vlg = cardGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 4f;
            vlg.padding = new RectOffset(10, 10, 8, 8);
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var level = NewText(cardGo.transform, $"{id}", 24, TextAnchor.UpperLeft);
            var stats = NewText(cardGo.transform, "", 18, TextAnchor.UpperLeft);
            stats.GetComponent<Text>().lineSpacing = 1.2f;

            var buttons = NewRow(cardGo.transform, "Buttons");
            var (upgradeBtn, upgradeLabel) = NewButton(buttons, "Upgrade", 20);
            upgradeBtn.onClick.AddListener(() => UpgradeClicked?.Invoke(id));

            var (tapBtn, _) = NewButton(buttons, "Tap", 20);
            tapBtn.onClick.AddListener(() => TapClicked?.Invoke(id));

            _cards[id] = new Card
            {
                LevelLabel = level.GetComponent<Text>(),
                StatsLabel = stats.GetComponent<Text>(),
                UpgradeButton = upgradeBtn,
                UpgradeLabel = upgradeLabel,
                TapButton = tapBtn,
            };
        }

        private RectTransform NewRow(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup));
            go.transform.SetParent(parent, false);
            var hlg = go.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 12f;
            hlg.childControlWidth = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = false;
            return (RectTransform)go.transform;
        }

        private GameObject NewText(Transform parent, string text, int size, TextAnchor anchor)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = _font;
            t.fontSize = size;
            t.alignment = anchor;
            t.color = Color.white;
            t.text = text;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            return go;
        }

        private (Button button, Text label) NewButton(Transform parent, string text, int size)
        {
            var go = new GameObject(text, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = new Color(0.2f, 0.5f, 0.9f, 0.9f);
            var btn = go.GetComponent<Button>();
            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelGo.transform.SetParent(go.transform, false);
            var label = labelGo.GetComponent<Text>();
            label.font = _font;
            label.fontSize = size;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = text;
            label.raycastTarget = false;
            var lrt = (RectTransform)labelGo.transform;
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            return (btn, label);
        }

        private struct Card
        {
            public Text LevelLabel;
            public Text StatsLabel;
            public Button UpgradeButton;
            public Text UpgradeLabel;
            public Button TapButton;
        }
    }
}
