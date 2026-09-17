# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-09-17

### Added

- **Runtime** `Loc` core: binary load, `Get` / `SetLanguage` / `OnLanguageChanged`, `{0}` format args
- **Editor** Tools → Localization Compiler (UTF-8 CSV → `.bytes`)
- **Sample uGUI**: `LocText` (legacy `UnityEngine.UI.Text`), `LocDemo` click-to-switch language demo
- **Sample UI Toolkit**: `LocLabel`, `LocDemoUITK`, `LocDemo.uxml` / `LocDemo.uss`
- Sample `Localization.csv` with zh_CN / en_US / ja_JP / ko_KR

### Notes

- Core has zero UI dependency; bindings live only in Samples
- Unity 6: demo uses `LegacyRuntime.ttf` (Arial.ttf removed upstream)
