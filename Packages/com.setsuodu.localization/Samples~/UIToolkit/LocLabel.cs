using UnityEngine.UIElements;
using Localization;

namespace Localization.Samples.UIToolkit
{
    /// <summary>
    /// Sample implementation for UI Toolkit.
    /// Custom Label that auto-refreshes on language change.
    /// Copy into your project and adapt as needed.
    /// </summary>
    public class LocLabel : Label
    {
        string _key;

        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                Refresh();
            }
        }

        public LocLabel() : this(string.Empty) { }

        public LocLabel(string key)
        {
            _key = key;
            RegisterCallback<AttachToPanelEvent>(OnAttach);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
            Refresh();
        }

        void OnAttach(AttachToPanelEvent e)
        {
            Loc.OnLanguageChanged += Refresh;
            Refresh();
        }

        void OnDetach(DetachFromPanelEvent e)
        {
            Loc.OnLanguageChanged -= Refresh;
        }

        public void Refresh()
        {
            if (!string.IsNullOrEmpty(_key))
                text = Loc.Get(_key);
        }

        public void SetKey(string key, params object[] args)
        {
            _key = key;
            text = Loc.Get(_key, args);
        }

        public new class UxmlFactory : UxmlFactory<LocLabel, UxmlTraits> { }

        public new class UxmlTraits : Label.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _keyAttr =
                new UxmlStringAttributeDescription { name = "key" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var label = (LocLabel)ve;
                label.Key = _keyAttr.GetValueFromBag(bag, cc);
            }
        }
    }
}
