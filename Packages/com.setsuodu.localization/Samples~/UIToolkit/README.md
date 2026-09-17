# UI Toolkit Sample

## Contents

- `LocLabel.cs` — custom VisualElement（继承 Label，语言切换自动刷新）
- `LocDemoUITK.cs` — MonoBehaviour 完整 Demo
- `LocDemo.uxml` / `LocDemo.uss` — 界面与样式
- `Localization.csv` — sample data（含 lang.*）

## 快速体验

1. Package Manager 导入本 Sample
2. **Tools → Localization Compiler** 编译 `Localization.csv` → 例如 `Assets/Resources/Localization.bytes`
3. 空场景新建 GameObject，添加 **UI Document** + **LocDemoUITK**
4. 在 LocDemoUITK 上：
   - 拖入 `Localization.bytes`（或放到 Resources 命名为 Localization）
   - （推荐）Source Asset 选 `LocDemo.uxml`，Style Sheet 选 `LocDemo.uss`
5. **Play** → 点击底部语言按钮，标题 / 按钮 / 步数 Label 实时切换

> 若不指定 UXML，`LocDemoUITK` 会在代码里动态构建同样界面。

## 手动用法

### UXML

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements"
         xmlns:loc="Localization.Samples.UIToolkit">
  <loc:LocLabel key="ui.home.title" />
</ui:UXML>
```

### C#

```csharp
var label = new LocLabel("ui.home.title");
root.Add(label);

label.SetKey("ui.game.steps", 12);
Loc.SetLanguage("en_US"); // 所有 LocLabel 自动刷新
```

### 语言按钮

```csharp
foreach (var lang in new[] { "zh_CN", "en_US", "ja_JP", "ko_KR" })
{
    var btn = new Button(() => Loc.SetLanguage(lang));
    btn.Add(new LocLabel("lang." + lang));
    root.Add(btn);
}
```
