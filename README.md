English | [Polski](README.pl.md)

# Etykiety IT

Etykiety IT is a Windows application for creating and printing IT asset labels
with QR codes, organization profiles, printer calibration, and print history.

> Current release: `3.0.0`

## Features

- organization profiles with independent Asset ID numbering,
- configurable Asset ID prefix and number of digits,
- built-in and user-defined label profiles,
- multi-label layouts,
- print preview,
- per-printer X/Y calibration,
- optional QR code containing the exact formatted Asset ID,
- dynamic QR sizing with a minimum of 4 dots per module,
- printability preflight that blocks unreadable or impossible combinations of
  printer, label profile, and content,
- persistent print history with search and filtering,
- export of visible history records to CSV and XLSX,
- Standard mode and Portable mode,
- High DPI support using PerMonitorV2.

## Verified printers

Physical print, geometry, calibration, QR, and printability preflight tests were
completed on:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

Etykiety IT uses the standard Windows printing system and may work with other
printers installed in Windows. Other printer models were not included in the
full v3.0.0 hardware validation.

Detailed hardware test results are available in
[`docs/printing-validation.md`](docs/printing-validation.md).
The QR design and validation rules are documented in
[`docs/qr.md`](docs/qr.md) and
[`docs/printability-preflight.md`](docs/printability-preflight.md).

## Download

Download the current release from [GitHub Releases](https://github.com/krzymianowski/EtykietyIT/releases).

Two Windows x64 archives are provided:

- **Standard** — stores user data in the current Windows user profile,
- **Portable** — stores user data next to the application.

Both variants contain the same self-contained application. The Portable archive
differs only by the presence of `portable.mode`.

## Standard vs Portable

In **Standard mode**, user data is stored in:

```text
%LOCALAPPDATA%\EtykietyIT\v3
```

In **Portable mode**, user data is stored in:

```text
<application directory>\Data\v3
```

Portable mode can be enabled with the `--portable` argument or by placing an
empty `portable.mode` file next to the executable. The application directory
must be writable. If it is not writable, the application reports an error and
does not silently fall back to Standard mode.

More information is available in
[`docs/portable-mode.md`](docs/portable-mode.md).

## Requirements

- Windows x64 supported by .NET 10,
- the Windows printing system,
- an installed driver for the selected printer.

The release archives are self-contained. Running Etykiety IT does not require:

- .NET Desktop Runtime,
- .NET SDK,
- Microsoft Excel for XLSX export.

## Unsigned binaries

The Etykiety IT 3.0.0 binaries are not digitally signed. Microsoft Defender
SmartScreen may display a warning when running the application for the first
time. Published release archives include SHA-256 checksums for integrity
verification.

## Build from source

Building requires Windows and the .NET SDK version specified in
[`global.json`](global.json). From the repository root, run:

```powershell
dotnet restore
dotnet build
dotnet test
```

The project uses Windows Forms and `System.Drawing.Printing`. The complete
release packaging process is documented in
[`docs/release.md`](docs/release.md), and development notes are available in
[`docs/development.md`](docs/development.md).

## Project structure

```text
EtykietyIT/             Windows Forms application
  Bootstrap/            manual dependency composition
  Export/               CSV/XLSX print history export
  Forms/                application forms
  Models/               data models
  Persistence/          application paths and JSON persistence
  Printing/             physically verified printing engine
  Resources/Profiles/   built-in label profiles
  Services/             application services
EtykietyIT.Tests/       automated tests
docs/                   technical and maintainer documentation
legacy/                 reference PowerShell 2.4 implementation
```

## Repository links

- Repository: [github.com/krzymianowski/EtykietyIT](https://github.com/krzymianowski/EtykietyIT)
- Releases: [github.com/krzymianowski/EtykietyIT/releases](https://github.com/krzymianowski/EtykietyIT/releases)
- Issues: [github.com/krzymianowski/EtykietyIT/issues](https://github.com/krzymianowski/EtykietyIT/issues)

Please use [GitHub Issues](https://github.com/krzymianowski/EtykietyIT/issues)
to report bugs or request changes.

## License

Etykiety IT is available under the MIT License. See [`LICENSE`](LICENSE).
Information about third-party components is available in
[`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md).
