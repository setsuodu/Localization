# Excel Localization (OpenUPM Package)

Pure Excel-driven localization **core** for Unity.  
UI bindings live in Samples, not core.

## Package

```
Packages/com.setsuodu.localization/
├── Runtime/          ← Loc.cs only (core)
├── Editor/           ← CSV → .bytes compiler
├── LICENSE.md
├── CHANGELOG.md
└── Samples~/
    ├── uGUI/         ← LocText + LocDemo（legacy UI.Text，无 TMP）
    ├── UIToolkit/    ← LocLabel + LocDemoUITK + UXML/USS
    └── Basic/
```

## Install (OpenUPM)

```bash
openupm add com.setsuodu.localization
```

Or Package Manager → Add package from git URL:

```
https://github.com/setsuodu/Localization.git?path=Packages/com.setsuodu.localization
```

## 完整示例：点击切换语言

### uGUI

1. Package Manager 导入 **uGUI Demo (Legacy Text)**
2. **Tools → Localization Compiler** 编译 Sample 里的 `Localization.csv`
3. 输出到 `Assets/Resources/Localization.bytes`（或拖到 LocDemo 的字段）
4. 打开 Sample 场景或空场景挂 `LocDemo` → Play
5. 点击语言按钮切换（zh_CN / en_US / ja_JP / ko_KR）

### UI Toolkit

1. 导入 **UI Toolkit Demo** → 编译 CSV
2. 空物体加 **UI Document** + **LocDemoUITK**，指定 UXML/USS（可选）
3. Play → 点语言按钮切换

## Publish

```bash
git tag 1.0.0
git push origin 1.0.0
```

然后到 [OpenUPM](https://openupm.com/) 提交包名 `com.setsuodu.localization`。

## License

MIT
