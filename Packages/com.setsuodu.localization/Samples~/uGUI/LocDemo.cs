using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Localization;

namespace Localization.Samples.uGUI
{
    /// <summary>
    /// Complete runnable demo: click language buttons to switch UI texts.
    /// Uses legacy uGUI Text only (no TextMeshPro) so Chinese/Japanese/Korean display without custom fonts.
    /// Attach to any GameObject in an empty scene, assign the compiled .bytes TextAsset, then Play.
    /// </summary>
    public class LocDemo : MonoBehaviour
    {
        [Header("Localization Data")]
        [Tooltip("Compile Samples~/uGUI/Localization.csv via Tools → Localization Compiler, then assign the .bytes here (or put under Resources as Localization)")]
        [SerializeField] TextAsset localizationBytes;

        [Header("Optional overrides (leave empty to auto-create UI)")]
        [SerializeField] Transform uiRoot;

        LocText _title;
        LocText _btnPlay;
        LocText _btnSettings;
        LocText _steps;
        Text _currentLangLabel;
        readonly List<Button> _langButtons = new();

        void Start()
        {
            if (!InitLoc())
            {
                Debug.LogError("[LocDemo] Failed to init Loc. Assign localizationBytes or place Localization.bytes under Resources.");
                return;
            }

            if (uiRoot == null)
                BuildUI();

            RefreshAll();
            Loc.OnLanguageChanged += RefreshAll;
        }

        void OnDestroy()
        {
            Loc.OnLanguageChanged -= RefreshAll;
        }

        bool InitLoc()
        {
            if (Loc.IsInited) return true;

            byte[] bytes = null;
            if (localizationBytes != null)
                bytes = localizationBytes.bytes;
            else
            {
                var ta = Resources.Load<TextAsset>("Localization");
                if (ta != null) bytes = ta.bytes;
            }

            if (bytes == null || bytes.Length == 0) return false;

            Loc.Init(bytes);
            return Loc.IsInited;
        }

        void BuildUI()
        {
            var canvasGo = new GameObject("LocDemoCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            uiRoot = canvasGo.transform;

            var panel = CreatePanel(uiRoot, new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(420, 480));
            var vlg = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(24, 24, 24, 24);
            vlg.spacing = 14;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            _title = CreateLocText(panel, "ui.home.title", 36, FontStyle.Bold);
            _currentLangLabel = CreatePlainText(panel, "", 20, FontStyle.Normal);

            var playGo = CreateButton(panel, out _);
            _btnPlay = playGo.GetComponentInChildren<LocText>();
            _btnPlay.Key = "ui.home.btn_play";

            var settingsGo = CreateButton(panel, out _);
            _btnSettings = settingsGo.GetComponentInChildren<LocText>();
            _btnSettings.Key = "ui.home.btn_settings";

            _steps = CreateLocText(panel, "ui.game.steps", 22, FontStyle.Normal);
            _steps.SetKey("ui.game.steps", 12);

            CreatePlainText(panel, "— Language —", 16, FontStyle.Italic);

            string[] langs = { "zh_CN", "en_US", "ja_JP", "ko_KR" };
            string[] langKeys = { "lang.zh_CN", "lang.en_US", "lang.ja_JP", "lang.ko_KR" };

            var langRow = new GameObject("LangRow", typeof(RectTransform));
            langRow.transform.SetParent(panel, false);
            var hlg = langRow.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childForceExpandWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandHeight = true;
            var le = langRow.AddComponent<LayoutElement>();
            le.preferredHeight = 44;

            for (int i = 0; i < langs.Length; i++)
            {
                string lang = langs[i];
                string key = langKeys[i];

                var btnGo = CreateButton(langRow.transform, out var btn);
                var loc = btnGo.GetComponentInChildren<LocText>();
                loc.Key = key;

                btn.onClick.AddListener(() => Loc.SetLanguage(lang));
                _langButtons.Add(btn);
            }
        }

        void RefreshAll()
        {
            if (_title != null) _title.Refresh();
            if (_btnPlay != null) _btnPlay.Refresh();
            if (_btnSettings != null) _btnSettings.Refresh();
            if (_steps != null) _steps.SetKey("ui.game.steps", 12);

            if (_currentLangLabel != null)
                _currentLangLabel.text = "Current: " + Loc.CurrentLang;
        }

        static Font DefaultFont()
        {
            // Legacy Text uses Arial by default; on most platforms OS font fallback handles CJK.
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        static RectTransform CreatePanel(Transform parent, Vector2 anchor, Vector2 anchoredPos, Vector2 size)
        {
            var go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = anchor;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            go.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.16f, 0.92f);
            return rt;
        }

        static LocText CreateLocText(Transform parent, string key, int fontSize, FontStyle style)
        {
            var go = new GameObject("LocText_" + key, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var uiText = go.AddComponent<Text>();
            uiText.font = DefaultFont();
            uiText.fontSize = fontSize;
            uiText.fontStyle = style;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.color = Color.white;
            uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = fontSize + 16;

            var loc = go.AddComponent<LocText>();
            loc.Key = key;
            return loc;
        }

        static Text CreatePlainText(Transform parent, string text, int fontSize, FontStyle style)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var uiText = go.AddComponent<Text>();
            uiText.font = DefaultFont();
            uiText.text = text;
            uiText.fontSize = fontSize;
            uiText.fontStyle = style;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.color = new Color(0.85f, 0.85f, 0.9f);
            uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = fontSize + 12;
            return uiText;
        }

        static GameObject CreateButton(Transform parent, out Button button)
        {
            var go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = new Color(0.22f, 0.45f, 0.75f, 1f);
            button = go.GetComponent<Button>();

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 48;

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;

            var uiText = labelGo.AddComponent<Text>();
            uiText.font = DefaultFont();
            uiText.fontSize = 22;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.color = Color.white;
            uiText.horizontalOverflow = HorizontalWrapMode.Overflow;
            uiText.verticalOverflow = VerticalWrapMode.Overflow;

            labelGo.AddComponent<LocText>();

            return go;
        }
    }
}
