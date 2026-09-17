using UnityEngine;
using UnityEngine.UI;
using Localization;

namespace Localization.Samples.uGUI
{
    /// <summary>
    /// Sample implementation for legacy uGUI Text (UnityEngine.UI.Text).
    /// No TextMeshPro dependency — default Arial supports basic CJK via system fallback on most platforms.
    /// Copy into your project and adapt as needed.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class LocText : MonoBehaviour
    {
        [SerializeField] string key;
        [SerializeField] bool autoRefresh = true;

        Text _uiText;
        bool _bound;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                Refresh();
            }
        }

        void Awake()
        {
            _uiText = GetComponent<Text>();
        }

        void OnEnable()
        {
            if (autoRefresh && !_bound)
            {
                Loc.OnLanguageChanged += Refresh;
                _bound = true;
            }
            Refresh();
        }

        void OnDisable()
        {
            if (_bound)
            {
                Loc.OnLanguageChanged -= Refresh;
                _bound = false;
            }
        }

        public void Refresh()
        {
            if (string.IsNullOrEmpty(key)) return;
            Apply(Loc.Get(key));
        }

        public void SetKey(string newKey, params object[] args)
        {
            key = newKey;
            Apply(Loc.Get(key, args));
        }

        void Apply(string text)
        {
            if (_uiText != null)
                _uiText.text = text;
        }
    }
}
