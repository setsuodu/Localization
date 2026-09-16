# Excel Localization (OpenUPM Package)

Pure Excel-driven localization **core** for Unity.  
UI bindings live in Samples, not core.

## Package

```
Packages/com.setsuodu.localization/
├── Runtime/          ← Loc.cs only (core)
├── Editor/           ← CSV → .bytes compiler
└── Samples~/
    ├── uGUI/         ← LocText + LocDemo（点击切换语言完整示例）
    ├── UIToolkit/    ← LocLabel (VisualElement)
    └── Basic/
```

## 完整示例：点击切换语言

1. Package Manager 导入 **uGUI / TextMeshPro Demo**
2. **Tools → Localization Compiler** 编译 Sample 里的 `Localization.csv`
3. 输出到 `Assets/Resources/Localization.bytes`（或拖到 LocDemo 的字段）
4. 空场景挂 `LocDemo` → Play
5. 点击语言按钮，标题 / 开始游戏 / 设置 / 步数 Label 全部实时切换（zh_CN / en_US / ja_JP / ko_KR）

## Local test

Copy `Packages/com.setsuodu.localization` into a Unity project's `Packages/` folder.

## Publish

Tag `1.0.0` → submit to OpenUPM.

Package name: `com.setsuodu.localization`

## License

MIT
