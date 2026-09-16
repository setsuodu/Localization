# uGUI / TextMeshPro Sample

## Contents

- `LocText.cs` — MonoBehaviour for UnityEngine.UI.Text and TextMeshProUGUI
- `LocDemo.cs` — **完整可运行示例**：点击语言按钮切换标题、按钮、Label
- `Localization.csv` — sample data（含语言名称）

## 快速体验（推荐）

1. Package Manager → 导入本 Sample
2. 打开 **Tools → Localization Compiler**
3. 选择本目录下的 `Localization.csv`，编译输出例如：
   `Assets/Resources/Localization.bytes`
4. 新建空场景，挂一个空 GameObject，添加 `LocDemo` 组件
5. （可选）把编译好的 `.bytes` 拖到 `Localization Bytes` 字段
6. **Play** → 点击底部语言按钮即可实时切换：
   - 标题 `ui.home.title`
   - 按钮「开始游戏 / 设置」
   - 动态文本「步数：12」
   - 当前语言指示

`LocDemo` 会在运行时自动创建 Canvas / 按钮 / LocText，无需手动摆 UI。

## 手动用法

```csharp
// 1. 加载
var ta = Resources.Load<TextAsset>("Localization");
Loc.Init(ta.bytes);

// 2. 任意 Text / TMP 上挂 LocText，填 Key
// 3. 切换语言 → 所有 LocText 自动 Refresh
Loc.SetLanguage("en_US");
Loc.SetLanguage("ja_JP");

// 带参数
locText.SetKey("ui.game.steps", 12);
```

## 关键原理

`LocText` 在 `OnEnable` 时订阅 `Loc.OnLanguageChanged`，
`Loc.SetLanguage` 触发事件后全部自动刷新，无需手动遍历 UI。
