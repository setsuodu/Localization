# UI Toolkit Sample

## Contents

- `LocLabel.cs` — custom VisualElement (extends Label)
- `Localization.csv` — sample data（含 lang.* 语言名称）

## Usage

1. Import this sample via Package Manager
2. Copy `LocLabel.cs` into your Scripts folder
3. Compile CSV → `.bytes`, then `Loc.Init(bytes)`
4. In UXML:

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements"
         xmlns:loc="Localization.Samples.UIToolkit">
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

## 语言切换示例

```csharp
// 创建几个语言按钮
foreach (var lang in new[] { "zh_CN", "en_US", "ja_JP", "ko_KR" })
{
    var btn = new Button(() => Loc.SetLanguage(lang)) { text = Loc.Get("lang." + lang) };
    root.Add(btn);
}
// 所有 LocLabel 会因 OnLanguageChanged 自动刷新
```
