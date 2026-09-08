# WinRTOCR

A .NET desktop OCR console application that uses the Windows Runtime **`Windows.Media.Ocr`** API to recognize text in images — no third-party OCR engine required.

## Features

- Command-line OCR for a single image file (jpg/png/bmp/tiff…)
- Outputs recognized text as **JSON** (`System.Text.Json`) to a file
- `-l` lists all OCR languages installed on the current Windows system
- Uses the input method language by default when no language is specified

## Requirements

- Windows 10 / 11 (the WinRT OCR engine is a built-in Windows component)
- .NET Framework 4.8 (usually pre-installed on Windows 10/11)
- Visual Studio 2019 / 2022 (or the .NET SDK) to build
- Windows **OCR language packs** for the languages you want to recognize
  (see *Settings → Time & Language → Language*; `WinRTOCR.exe -l` shows what is installed)

## Build

Open `WinRTOCR.sln` in Visual Studio and build, or run:

```
dotnet build WinRTOCR.sln
```

Output: `WinRTOCR\bin\Debug\net48\WinRTOCR.exe` (or `Release\net48`).

## Usage

```
WinRTOCR.exe <imagePath> [language] [outputPath]
```

| Argument     | Default                    | Description                                   |
| ------------ | -------------------------- | --------------------------------------------- |
| `imagePath`  | `image.jpg`                | Path to the image to OCR                      |
| `language`   | input method language      | BCP-47 language tag, e.g. `zh-Hans-CN`, `en-US` |
| `outputPath` | `out.json`                 | File the JSON result is written to            |

Options:

- `-h` – show help
- `-l` – list available (installed) OCR languages and exit

Examples:

```
WinRTOCR.exe image.jpg zh-Hans-CN out.json
WinRTOCR.exe scan.png en-US
WinRTOCR.exe -l
```

> Note: `imagePath`/`language`/`outputPath` are only applied when exactly 3 arguments are given; otherwise the defaults are used (the first argument is always treated as the image path).

## Project layout

- SDK-style project targeting **.NET Framework 4.8** (`net48`)
- WinRT metadata is provided by the NuGet package **`Microsoft.Windows.SDK.Contracts`** (no Windows SDK installation needed, no hard-coded `Windows.winmd` path)
- JSON serialization via the `System.Text.Json` NuGet package
- Dependencies are restored through `PackageReference` (no `packages.config`)

## How to call WinRT APIs from a .NET desktop app

There are many (often confusing) posts about this topic:

* [Create a Universal Windows Platform console app](https://docs.microsoft.com/zh-cn/windows/uwp/launch-resume/console-uwp). This creates a UWP app which needs installation.
* [Call Windows Runtime APIs in desktop apps](https://docs.microsoft.com/zh-cn/windows/apps/desktop/modernize/desktop-to-uwp-enhance).
* [C#/WinRT](https://github.com/microsoft/cswinrt/blob/master/README.md).
* [How to use Windows.Devices.Geolocation API in a C# WinForm Win32 Desktop application in Windows 10?](https://social.msdn.microsoft.com/Forums/windows/en-US/40b4f86d-b6d5-430e-a235-7fc003dec4c4/how-to-use-windowsdevicesgeolocation-api-in-a-c-winform-win32-desktop-application-in-windows-10).

The approach that works here: reference the **`Microsoft.Windows.SDK.Contracts`** NuGet package from an SDK-style (`.NET Framework 4.8`) console project. The WinRT projection types (`Windows.Media.Ocr`, `Windows.Storage`, …) then resolve at compile time and run correctly on Windows 10/11 without a UWP container.




