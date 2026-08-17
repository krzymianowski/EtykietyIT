# Contributing to Etykiety IT

## Before You Start

- Report a reproducible bug through the
  [Bug report form](https://github.com/krzymianowski/EtykietyIT/issues/new?template=bug_report.yml).
- Propose an improvement through the
  [Feature request form](https://github.com/krzymianowski/EtykietyIT/issues/new?template=feature_request.yml).
- Report a security issue using
  [Private Vulnerability Reporting](https://github.com/krzymianowski/EtykietyIT/security/advisories/new),
  not a public GitHub Issue.

Reports and contributions in English are preferred. Polish is also welcome.

## Development Environment

You need:

- Windows,
- the .NET SDK selected by [`global.json`](global.json),
- Git.

Visual Studio is optional. It is not required to build or test the project.

From the repository root, run:

```powershell
dotnet restore
dotnet build
dotnet test
```

## Project Principles

- Prefer small, focused changes.
- Avoid unrelated refactoring.
- Preserve backward compatibility of persisted data unless a migration is
  explicitly designed and tested.
- Do not mix feature work with formatting or refactoring without a clear
  reason.
- Add or update tests for behavior changes.

## Printing Code — Important

Printing geometry is hardware-sensitive. It has been physically validated on:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

Changes involving any of the following are high risk:

- `HardMargin`,
- `Bounds`,
- `VisibleClipBounds`,
- `TranslateTransform`,
- paper selection,
- printer calibration,
- QR geometry,
- printability preflight.

Do not refactor these areas incidentally. Changes to printing geometry should
include:

- automated tests,
- print preview validation,
- physical printer validation where applicable.

## Tests

Before opening a pull request, run:

```powershell
dotnet build
dotnet test
```

The expected result is:

- 0 build errors,
- 0 warnings,
- all tests passing.

If the change affects printing, describe the manual and hardware tests that
were performed.

## Pull Requests

A pull request should explain:

- what changed,
- why the change is needed,
- how it was tested,
- screenshots for UI changes where useful,
- the hardware and printer used for printing changes.

Prefer one logical problem or feature per pull request.

## Coding Style

Follow [`.editorconfig`](.editorconfig) and the naming and formatting already
used in the project. Keep code simple and avoid unnecessary abstractions.

## License

Contributions are submitted under the project's [MIT License](LICENSE).
