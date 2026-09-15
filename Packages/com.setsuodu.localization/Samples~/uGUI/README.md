# uGUI / TextMeshPro Sample

## Contents

- `LocText.cs` — MonoBehaviour for UnityEngine.UI.Text and TextMeshProUGUI
- `Localization.csv` — sample data

## Usage

1. Import this sample via Package Manager
2. Copy `LocText.cs` into your Scripts folder (or keep reference)
3. Compile `Localization.csv` with **Tools → Excel Localization Compiler**
4. Load bytes: `Loc.Init(bytes)`
5. Attach `LocText` to any Text / TMP, fill **Key**

```csharp
// Dynamic
locText.SetKey("ui.game.steps", 12);
Loc.SetLanguage("en_US"); // all LocText auto refresh
```
