# LocalizationResourceManager.Maui.Aot

A **NativeAOT-friendly** fork of [Johan Svensson's LocalizationResourceManager.Maui](https://github.com/SirJohnK/LocalizationResourceManager.Maui).

## What's different

The `TranslateExtension` has been patched to use `LocalizedString.Localized` (a named property) instead of `Path="[key]"` (an indexer). This fixes MAUI's binding engine under **NativeAOT / full trimming**, where indexer reflection via `GetDefaultMembers()` fails.

## Why this fork

When publishing a .NET MAUI app with NativeAOT (or aggressive trimming), the original `TranslateExtension` throws a runtime exception because the binding engine cannot resolve indexer access via reflection. This fork provides a drop-in fix.

## Usage

```xml
<PackageReference Include="LocalizationResourceManager.Maui.Aot" Version="1.0.1" />
```

Usage is identical to the original:

```xml
<Label Text="{Binding [WelcomeMessage], Source={StaticResource LocalizationResourceManager}}" />
```

## Credits

All credit to **Johan Svensson**, Charlin Agramonte, Brandon Minnick, and Maksym Koshovyi for the original LocalizationResourceManager.Maui library.
