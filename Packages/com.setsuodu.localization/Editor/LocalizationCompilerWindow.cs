using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    /// <summary>
    /// Minimal compiler: CSV (exported from Localization.xlsx) → .bytes
    /// No EPPlus dependency. Users export Excel as UTF-8 CSV first.
    /// </summary>
    public class LocalizationCompilerWindow : EditorWindow
    {
        string _csvPath = "";
        string _outputPath = "Assets/Bundles/Localization/Localization.bytes";
        Vector2 _scroll;
        string _log = "";

        [MenuItem("Tools/Localization Compiler")]
        public static void Open()
        {
            var win = GetWindow<LocalizationCompilerWindow>("Localization");
            win.minSize = new Vector2(480, 320);
        }

        void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Localization Compiler", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. Open Localization.xlsx in Excel / WPS\n" +
                "2. Save / Export as UTF-8 CSV\n" +
                "3. Select the CSV below and compile to .bytes",
                MessageType.Info);

            EditorGUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            _csvPath = EditorGUILayout.TextField("CSV Path", _csvPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string p = EditorUtility.OpenFilePanel("Select Localization CSV", "", "csv");
                if (!string.IsNullOrEmpty(p)) _csvPath = p;
            }
            EditorGUILayout.EndHorizontal();

            _outputPath = EditorGUILayout.TextField("Output .bytes", _outputPath);

            EditorGUILayout.Space(8);
            if (GUILayout.Button("Compile", GUILayout.Height(32)))
                Compile();

            EditorGUILayout.Space(8);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.TextArea(_log, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        void Compile()
        {
            _log = "";
            try
            {
                if (string.IsNullOrEmpty(_csvPath) || !File.Exists(_csvPath))
                {
                    LogError("CSV file not found.");
                    return;
                }

                var lines = File.ReadAllLines(_csvPath, Encoding.UTF8);
                if (lines.Length < 2)
                {
                    LogError("CSV needs at least header + 1 data row.");
                    return;
                }

                var header = SplitCsvLine(lines[0]);
                if (header.Count < 2 || !header[0].Equals("Key", StringComparison.OrdinalIgnoreCase))
                {
                    LogError("First column must be 'Key'.");
                    return;
                }

                var languages = new List<string>();
                for (int i = 1; i < header.Count; i++)
                {
                    string lang = header[i].Trim();
                    if (!string.IsNullOrEmpty(lang))
                        languages.Add(lang);
                }

                if (languages.Count == 0)
                {
                    LogError("No language columns found.");
                    return;
                }

                var table = new List<(string key, string[] texts)>();
                for (int r = 1; r < lines.Length; r++)
                {
                    if (string.IsNullOrWhiteSpace(lines[r])) continue;
                    var cols = SplitCsvLine(lines[r]);
                    if (cols.Count == 0) continue;

                    string key = cols[0].Trim();
                    if (string.IsNullOrEmpty(key)) continue;

                    var texts = new string[languages.Count];
                    for (int l = 0; l < languages.Count; l++)
                    {
                        int colIdx = l + 1;
                        texts[l] = colIdx < cols.Count ? cols[colIdx] : "";
                    }
                    table.Add((key, texts));
                }

                string outFull = _outputPath;
                if (!Path.IsPathRooted(outFull))
                    outFull = Path.GetFullPath(Path.Combine(Application.dataPath, "..", _outputPath));

                string dir = Path.GetDirectoryName(outFull);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms, Encoding.UTF8))
                {
                    bw.Write((byte)'L');
                    bw.Write((byte)'O');
                    bw.Write((byte)'C');
                    bw.Write((byte)'B');
                    bw.Write(1);
                    bw.Write(languages.Count);
                    foreach (var lang in languages)
                        WriteString(bw, lang);
                    bw.Write(table.Count);
                    foreach (var (key, texts) in table)
                    {
                        WriteString(bw, key);
                        for (int i = 0; i < languages.Count; i++)
                            WriteString(bw, texts[i]);
                    }
                    File.WriteAllBytes(outFull, ms.ToArray());
                }

                Log($"OK  keys={table.Count}  langs=[{string.Join(", ", languages)}]");
                Log($"→ {outFull}");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                LogError(e.ToString());
            }
        }

        static void WriteString(BinaryWriter bw, string s)
        {
            if (s == null)
            {
                bw.Write(-1);
                return;
            }
            byte[] utf8 = Encoding.UTF8.GetBytes(s);
            bw.Write(utf8.Length);
            bw.Write(utf8);
        }

        static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                        inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                    sb.Append(c);
            }
            result.Add(sb.ToString());
            return result;
        }

        void Log(string msg) => _log += msg + "\n";
        void LogError(string msg)
        {
            _log += "[ERROR] " + msg + "\n";
            Debug.LogError("[Localization] " + msg);
        }
    }
}
