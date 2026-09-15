# Excel Localization (OpenUPM Package)

Pure Excel-driven localization **core** for Unity.  
UI bindings live in Samples, not core.

## Package

```
Packages/com.setsuodu.excel-localization/
├── Runtime/          ← Loc.cs only (core)
├── Editor/           ← CSV → .bytes compiler
└── Samples~/
    ├── uGUI/         ← LocText (Text / TMP)
    └── UIToolkit/    ← LocLabel (VisualElement)
```

## Local test

Copy `Packages/com.setsuodu.excel-localization` into a Unity project's `Packages/` folder.

## Publish

Tag `1.0.0` → submit to OpenUPM.

Package name: `com.setsuodu.excel-localization`

## License

MIT
