# Excel Localization

Pure Excel-driven localization **core** for Unity.  
**No ScriptableObject.** Key routing. Binary format ready for hot-update.

UI bindings (`LocText` / `LocLabel`) are provided as **Samples**, not core.

## Install

```bash
openupm add com.setsuodu.excel-localization
```

Or Package Manager → Add package from disk / git.

## Core API

```csharp
// Load binary (Resources / Addressables / HotUpdate)
Loc.Init(bytes);

// Get text
string title = Loc.Get("ui.home.title");
string steps = Loc.Get("ui.game.steps", 12);

// Switch language → fires OnLanguageChanged
Loc.SetLanguage("en_US");

// Event for auto-refresh
Loc.OnLanguageChanged += () => { /* refresh your UI */ };
```

## Excel / CSV Format

| Key | zh_CN | en_US | ja_JP | ko_KR |
|-----|-------|-------|-------|-------|
| ui.home.title | 主界面 | Home | ホーム | 홈 |
| ui.game.steps | 步数：{0} | Steps: {0} | 残り手数：{0} | 남은 수: {0} |

- First column **must** be `Key`
- Other columns = language codes
- Supports `{0}` `{1}` placeholders

## Editor

**Tools → Excel Localization Compiler**

1. Export Localization.xlsx as UTF-8 CSV  
2. Select CSV → compile to `.bytes`

## Samples

Import via Package Manager → Samples:

| Sample | Content |
|--------|---------|
| **uGUI / TextMeshPro Demo** | `LocText` MonoBehaviour + sample CSV |
| **UI Toolkit Demo** | `LocLabel` VisualElement + sample CSV |

Copy the sample scripts into your project and adapt as needed.

## Binary Format

```
MAGIC    4  'L''O''C''B'
VERSION  4  int32 = 1
LANG_CNT 4
LANG×N   string
KEY_CNT  4
KEY×N    string(key) + string×LANG_CNT
```

string = int32 length (-1=null) + UTF-8

## License

MIT
