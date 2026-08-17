# Etykiety IT

Etykiety IT to niewielka aplikacja Windows do tworzenia i drukowania etykiet
inwentarzowych dla urządzeń i zasobów IT.

> Status projektu: development — `3.0.0-dev`

## Funkcje

- automatyczna numeracja Asset ID,
- niezależne profile organizacji z własną numeracją i ustawieniami,
- wbudowane i własne profile etykiet,
- obsługa drukarek dostępnych w systemie Windows,
- osobna kalibracja X/Y dla każdej drukarki,
- podgląd przed wydrukiem,
- opcjonalny QR zawierający dokładnie sformatowany Asset ID, z dynamicznym
  dopasowaniem wielkości i minimum 4 dots/module,
- preflight blokujący podgląd i druk nieczytelnych kombinacji profilu,
  drukarki i treści,
- trwała historia przekazanych zadań drukowania,
- wyszukiwanie i filtrowanie historii,
- eksport widocznych rekordów do CSV i XLSX,
- tryb danych Standard oraz Portable.

## Wymagania

- Windows,
- .NET 10 Desktop Runtime do uruchomienia aplikacji,
- .NET 10 SDK do budowania projektu.

Aplikacja używa standardowego systemu drukowania Windows i nie jest ograniczona
wyłącznie do urządzeń DYMO. Fizyczne testy geometrii i kalibracji wykonano na:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

Szczegóły testów sprzętowych znajdują się w
[`docs/printing-validation.md`](docs/printing-validation.md).
Obsługa QR i preflight zostały fizycznie zweryfikowane na DYMO LabelWriter 450
i DYMO LabelWriter 550, w tym dla układu 2 × 2 / 4 małe etykiety. Szczegóły
opisują [`docs/qr.md`](docs/qr.md) i
[`docs/printing-validation.md`](docs/printing-validation.md).

## Dane aplikacji

W trybie Standard dane użytkownika znajdują się w:

```text
%LOCALAPPDATA%\EtykietyIT\v3
```

Tryb Portable można włączyć parametrem `--portable` albo plikiem
`portable.mode` umieszczonym obok pliku wykonywalnego. Dane są wtedy zapisywane
w katalogu:

```text
<katalog aplikacji>\Data\v3
```

Katalog trybu Portable musi być zapisywalny. Aplikacja nie przełącza się
automatycznie na tryb Standard, gdy zapis jest niemożliwy. Więcej informacji:
[`docs/portable-mode.md`](docs/portable-mode.md).

## Budowanie i testy

W katalogu repozytorium uruchom:

```powershell
dotnet restore
dotnet build
dotnet test
```

Projekt jest przeznaczony dla Windows i wykorzystuje Windows Forms oraz
`System.Drawing.Printing`.

## Struktura projektu

```text
EtykietyIT/             aplikacja WinForms
  Bootstrap/            ręczne składanie zależności
  Export/               eksport historii CSV/XLSX
  Forms/                formularze aplikacji
  Models/               modele danych
  Persistence/          ścieżki i zapis JSON
  Printing/             zweryfikowany silnik drukowania
  Resources/Profiles/   wbudowane profile etykiet
  Services/             logika aplikacyjna
EtykietyIT.Tests/       testy automatyczne
docs/                   dokumentacja techniczna
legacy/                 referencyjna wersja PowerShell 2.4
```

Wskazówki dla osób rozwijających projekt znajdują się w
[`docs/development.md`](docs/development.md).

## Licencja

Projekt jest udostępniany na licencji MIT. Zobacz plik [`LICENSE`](LICENSE).
Informacje o użytych bibliotekach znajdują się w
[`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md).
