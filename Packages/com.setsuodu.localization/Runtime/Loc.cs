using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Localization
{
    /// <summary>
    /// Pure Excel-driven localization runtime (core only).
    /// No UI binding. No ScriptableObject.
    /// </summary>
    public static class Loc
    {
        public static string CurrentLang { get; private set; } = "zh_CN";

        /// <summary>Fired after language is changed. Samples subscribe to this.</summary>
        public static event Action OnLanguageChanged;

        static readonly Dictionary<string, Dictionary<string, string>> _table = new();
        static readonly List<string> _languages = new();
        static bool _inited;

        public static IReadOnlyList<string> Languages => _languages;
        public static bool IsInited => _inited;

        /// <summary>Load from binary produced by the Editor compiler.</summary>
        public static void Init(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 12)
            {
                Debug.LogError("[Loc] Invalid bytes.");
                return;
            }

            _table.Clear();
            _languages.Clear();

            var reader = new LocBinaryReader(bytes);
            if (!reader.ReadHeader())
            {
                Debug.LogError("[Loc] Bad magic or version.");
                return;
            }

            int langCount = reader.ReadInt32();
            for (int i = 0; i < langCount; i++)
                _languages.Add(reader.ReadString() ?? "");

            int keyCount = reader.ReadInt32();
            for (int i = 0; i < keyCount; i++)
            {
                string key = reader.ReadString() ?? "";
                var langDict = new Dictionary<string, string>(langCount);
                for (int l = 0; l < langCount; l++)
                {
                    string text = reader.ReadString() ?? "";
                    langDict[_languages[l]] = text;
                }
                _table[key] = langDict;
            }

            _inited = true;

            string sys = Application.systemLanguage.ToString();
            if (_languages.Contains(sys))
                CurrentLang = sys;
            else if (_languages.Count > 0 && !_languages.Contains(CurrentLang))
                CurrentLang = _languages[0];

            Debug.Log($"[Loc] Loaded {_table.Count} keys, {_languages.Count} languages. Current={CurrentLang}");
        }

        /// <summary>Get localized text. Missing key returns [key] for easy debug.</summary>
        public static string Get(string key, params object[] args)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            if (!_table.TryGetValue(key, out var langDict))
                return $"[{key}]";

            if (!langDict.TryGetValue(CurrentLang, out var text) || string.IsNullOrEmpty(text))
            {
                if (!langDict.TryGetValue("zh_CN", out text) || string.IsNullOrEmpty(text))
                {
                    if (_languages.Count > 0)
                        langDict.TryGetValue(_languages[0], out text);
                }
                if (string.IsNullOrEmpty(text))
                    return $"[{key}]";
            }

            if (args != null && args.Length > 0)
            {
                try { return string.Format(text, args); }
                catch { return text; }
            }
            return text;
        }

        public static void SetLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang) || lang == CurrentLang)
                return;
            if (_languages.Count > 0 && !_languages.Contains(lang))
            {
                Debug.LogWarning($"[Loc] Language '{lang}' not found. Available: {string.Join(", ", _languages)}");
                return;
            }

            CurrentLang = lang;
            OnLanguageChanged?.Invoke();
        }

        public static bool Has(string key) => _table.ContainsKey(key);

        public static void Clear()
        {
            _table.Clear();
            _languages.Clear();
            _inited = false;
            CurrentLang = "zh_CN";
        }
    }

    /// <summary>Minimal binary reader for Loc format.</summary>
    public ref struct LocBinaryReader
    {
        readonly byte[] _buf;
        int _pos;

        public LocBinaryReader(byte[] buf)
        {
            _buf = buf;
            _pos = 0;
        }

        public bool ReadHeader()
        {
            if (_buf.Length < 8) return false;
            if (_buf[0] != (byte)'L' || _buf[1] != (byte)'O' ||
                _buf[2] != (byte)'C' || _buf[3] != (byte)'B')
                return false;
            _pos = 4;
            int ver = ReadInt32();
            return ver == 1;
        }

        public int ReadInt32()
        {
            int v = _buf[_pos] | (_buf[_pos + 1] << 8) | (_buf[_pos + 2] << 16) | (_buf[_pos + 3] << 24);
            _pos += 4;
            return v;
        }

        public string ReadString()
        {
            int len = ReadInt32();
            if (len < 0) return null;
            if (len == 0) return "";
            string s = Encoding.UTF8.GetString(_buf, _pos, len);
            _pos += len;
            return s;
        }
    }
}
