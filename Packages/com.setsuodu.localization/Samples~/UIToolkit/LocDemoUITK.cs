using UnityEngine;
using UnityEngine.UIElements;
using Localization;

namespace Localization.Samples.UIToolkit
{
    /// <summary>
    /// UI Toolkit demo: load localization, show LocLabels, click language buttons to switch.
    /// Attach to a GameObject that has UIDocument. Assign Source Asset (LocDemo.uxml) and Style Sheet (LocDemo.uss).
    /// Or leave Source Asset empty — demo will build UI in code.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LocDemoUITK : MonoBehaviour
    {
        [Header("Localization Data")]
        [Tooltip("Compile Localization.csv via Tools → Localization Compiler, then assign .bytes (or put under Resources as Localization)")]
        [SerializeField] TextAsset localizationBytes;

        [Header("Optional — if empty, UI is built in code")]
        [SerializeField] VisualTreeAsset uxml;
        [SerializeField] StyleSheet uss;

        UIDocument _doc;
        Label _currentLang;
        LocLabel _steps;

        static readonly string[] Langs = { "zh_CN", "en_US", "ja_JP", "ko_KR" };

        void OnEnable()
        {
            if (!InitLoc())
            {
                Debug.LogError("[LocDemoUITK] Failed to init Loc. Assign localizationBytes or place Localization.bytes under Resources.");
                return;
            }

            _doc = GetComponent<UIDocument>();
            if (_doc == null) return;

            // Prefer assigned UXML; otherwise build in code
            if (uxml != null)
                _doc.visualTreeAsset = uxml;

            var root = _doc.rootVisualElement;
            if (root == null) return;

            if (uss != null)
                root.styleSheets.Add(uss);

            // If no UXML content, build programmatically
            if (root.Q("panel") == null)
                BuildUI(root);

            WireLanguageButtons(root);
            CacheDynamicLabels(root);
            RefreshDynamic();

            Loc.OnLanguageChanged += RefreshDynamic;
        }

        void OnDisable()
        {
            Loc.OnLanguageChanged -= RefreshDynamic;
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

        void WireLanguageButtons(VisualElement root)
        {
            foreach (var lang in Langs)
            {
                var btn = root.Q<Button>("lang-" + lang);
                if (btn == null) continue;

                // Capture for closure
                string code = lang;
                btn.clicked += () => Loc.SetLanguage(code);
            }
        }

        void CacheDynamicLabels(VisualElement root)
        {
            _currentLang = root.Q<Label>("current-lang");
            _steps = root.Q<LocLabel>("steps");
        }

        void RefreshDynamic()
        {
            if (_currentLang != null)
                _currentLang.text = "Current: " + Loc.CurrentLang;

            // steps needs format arg — LocLabel.Refresh alone has no args
            if (_steps != null)
                _steps.SetKey("ui.game.steps", 12);
        }

        /// <summary>Fallback when UXML is not assigned.</summary>
        void BuildUI(VisualElement root)
        {
            root.Clear();
            root.AddToClassList("root");

            var panel = new VisualElement { name = "panel" };
            panel.AddToClassList("panel");
            root.Add(panel);

            var title = new LocLabel("ui.home.title") { name = "title" };
            title.AddToClassList("title");
            panel.Add(title);

            _currentLang = new Label("Current: ") { name = "current-lang" };
            _currentLang.AddToClassList("current-lang");
            panel.Add(_currentLang);

            panel.Add(MakeLocButton("btn-play", "ui.home.btn_play"));
            panel.Add(MakeLocButton("btn-settings", "ui.home.btn_settings"));

            _steps = new LocLabel("ui.game.steps") { name = "steps" };
            _steps.AddToClassList("steps");
            panel.Add(_steps);

            var section = new Label("— Language —");
            section.AddToClassList("section");
            panel.Add(section);

            var row = new VisualElement { name = "lang-row" };
            row.AddToClassList("lang-row");
            panel.Add(row);

            foreach (var lang in Langs)
            {
                var btn = MakeLocButton("lang-" + lang, "lang." + lang);
                btn.AddToClassList("lang-btn");
                btn.RemoveFromClassList("btn");
                row.Add(btn);
            }

            // Inline minimal styles if no USS
            if (uss == null)
                ApplyInlineStyles(root);
        }

        static Button MakeLocButton(string name, string key)
        {
            var btn = new Button { name = name };
            btn.AddToClassList("btn");
            var label = new LocLabel(key);
            label.AddToClassList("btn-label");
            // Clear default button text so only LocLabel shows
            btn.text = "";
            btn.Add(label);
            return btn;
        }

        static void ApplyInlineStyles(VisualElement root)
        {
            root.style.flexGrow = 1;
            root.style.justifyContent = Justify.Center;
            root.style.alignItems = Align.Center;
            root.style.backgroundColor = new Color(0.12f, 0.12f, 0.14f);

            var panel = root.Q("panel");
            if (panel != null)
            {
                panel.style.width = 420;
                panel.style.paddingTop = panel.style.paddingBottom =
                    panel.style.paddingLeft = panel.style.paddingRight = 24;
                panel.style.backgroundColor = new Color(0.12f, 0.12f, 0.16f, 0.92f);
                panel.style.borderTopLeftRadius = panel.style.borderTopRightRadius =
                    panel.style.borderBottomLeftRadius = panel.style.borderBottomRightRadius = 12;
            }

            foreach (var btn in root.Query<Button>().ToList())
            {
                btn.style.height = 48;
                btn.style.marginBottom = 10;
                btn.style.backgroundColor = new Color(0.22f, 0.45f, 0.75f);
                btn.style.borderTopLeftRadius = btn.style.borderTopRightRadius =
                    btn.style.borderBottomLeftRadius = btn.style.borderBottomRightRadius = 6;
                btn.style.borderTopWidth = btn.style.borderBottomWidth =
                    btn.style.borderLeftWidth = btn.style.borderRightWidth = 0;
                btn.style.justifyContent = Justify.Center;
                btn.style.alignItems = Align.Center;
            }

            var title = root.Q("title");
            if (title != null)
            {
                title.style.fontSize = 36;
                title.style.unityFontStyleAndWeight = FontStyle.Bold;
                title.style.color = Color.white;
                title.style.unityTextAlign = TextAnchor.MiddleCenter;
                title.style.height = 48;
                title.style.marginBottom = 8;
            }

            var steps = root.Q("steps");
            if (steps != null)
            {
                steps.style.fontSize = 20;
                steps.style.color = Color.white;
                steps.style.unityTextAlign = TextAnchor.MiddleCenter;
                steps.style.height = 32;
                steps.style.marginTop = 8;
                steps.style.marginBottom = 16;
            }

            var langRow = root.Q("lang-row");
            if (langRow != null)
            {
                langRow.style.flexDirection = FlexDirection.Row;
                langRow.style.justifyContent = Justify.SpaceBetween;
                langRow.style.height = 44;

                foreach (var child in langRow.Children())
                {
                    child.style.flexGrow = 1;
                    child.style.marginLeft = child.style.marginRight = 4;
                    child.style.height = 44;
                    child.style.marginBottom = 0;
                }
            }
        }
    }
}
