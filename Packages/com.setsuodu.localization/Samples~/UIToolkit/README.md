# UI Toolkit Sample

## Contents

- `LocLabel.cs` — custom VisualElement (extends Label)
- `Localization.csv` — sample data

## Usage

1. Import this sample via Package Manager
2. Copy `LocLabel.cs` into your Scripts folder
3. Compile CSV → `.bytes`, then `Loc.Init(bytes)`
4. In UXML:

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements"
         xmlns:loc="ExcelLocalization.Samples.UIToolkit">
  <loc:LocLabel key="ui.home.title" />
</ui:UXML>
```

Or in C#:

```csharp
var label = new LocLabel("ui.home.title");
root.Add(label);

label.SetKey("ui.game.steps", 12);
Loc.SetLanguage("en_US"); // auto refresh
```
