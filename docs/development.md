# Rozwój Etykiety IT

## Środowisko

Projekt wymaga systemu Windows oraz .NET 10 SDK. Do pracy można użyć Visual
Studio z obsługą Windows Forms albo innego edytora obsługującego projekty C#.

## Budowanie i testowanie

W katalogu głównym repozytorium uruchom:

```powershell
dotnet restore
dotnet build
dotnet test
```

Workflow GitHub Actions wykonuje te same etapy na `windows-latest` w
konfiguracji Release.

## Dane aplikacji

Modele JSON, lokalizacje plików i zasady kompatybilności opisuje
[`data-formats.md`](data-formats.md). Tryb Portable opisuje
[`portable-mode.md`](portable-mode.md), a profile organizacji
[`organization-profiles.md`](organization-profiles.md).

Dane użytkownika nie powinny być zapisywane w repozytorium. Aplikacja używa
`%LOCALAPPDATA%\EtykietyIT\v3` w trybie Standard lub `Data\v3` obok aplikacji w
trybie Portable.

## Silnik drukowania

`Printing/LabelPrintJob.cs` jest migracją działającego silnika z
`legacy/EtykietyIT_v2.4.ps1`. Renderer oraz jego geometria zostały fizycznie
zweryfikowane na DYMO LabelWriter 450 i 550.

Nie należy zmieniać obliczeń `HardMargin`, `VisibleClipBounds`,
`PageSettings.Bounds`, `TranslateTransform`, orientacji ani kalibracji bez
ponownego wykonania testów sprzętowych. Potwierdzone wyniki znajdują się w
[`printing-validation.md`](printing-validation.md).

## Główne katalogi

- `Bootstrap/` — ręczne składanie serwisów bez frameworka DI,
- `Export/` — eksport historii CSV i XLSX,
- `Forms/` — formularze Windows Forms,
- `Models/` — wersjonowane modele danych,
- `Persistence/` — zapis JSON i wyznaczanie ścieżek,
- `Printing/` — silnik drukowania,
- `Services/` — logika aplikacyjna,
- `EtykietyIT.Tests/` — testy automatyczne.
