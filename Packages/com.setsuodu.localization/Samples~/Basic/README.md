# Basic Sample

1. Open **Tools → Excel Localization Compiler**
2. Select `Localization.csv` in this folder
3. Set output path, e.g. `Assets/Bundles/Localization/Localization.bytes`
4. Click **Compile**
5. In code:

```csharp
var ta = Resources.Load<TextAsset>("Localization"); // or your hot-update path
Loc.Init(ta.bytes);

Debug.Log(Loc.Get("ui.home.title"));
Loc.SetLanguage("en_US");
```

6. On any TextMeshProUGUI, add **LocText** component and fill Key.
