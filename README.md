# Etykiety IT

Etykiety IT to niewielka aplikacja Windows do tworzenia i drukowania etykiet
inwentarzowych dla urządzeń i zasobów IT.

> Wersja: `3.0.0`

## Linki

- Repozytorium: [github.com/krzymianowski/EtykietyIT](https://github.com/krzymianowski/EtykietyIT)
- Wydania: [GitHub Releases](https://github.com/krzymianowski/EtykietyIT/releases)
- Zgłaszanie problemów: [GitHub Issues](https://github.com/krzymianowski/EtykietyIT/issues)

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

## Dystrybucja

Etykiety IT 3.0.0 jest publikowany dla Windows x64 jako aplikacja
self-contained. Do jego uruchomienia nie trzeba instalować .NET Desktop
Runtime ani .NET SDK. Microsoft Excel nie jest wymagany do tworzenia plików
XLSX.

Powstają dwa warianty ZIP zbudowane z tych samych plików aplikacji:

- Standard — dane użytkownika są zapisywane w `%LOCALAPPDATA%`,
- Portable — zawiera plik `portable.mode`, a dane są zapisywane obok aplikacji.

Binaria Etykiety IT 3.0.0 nie są podpisane cyfrowo. Windows może wyświetlić
ostrzeżenie Microsoft Defender SmartScreen. Integralność paczek można
zweryfikować za pomocą opublikowanych sum SHA-256.

## Wymagania

- 64-bitowy Windows obsługiwany przez .NET 10,
- .NET SDK 10.0.400 wyłącznie do budowania projektu.

Aplikacja używa standardowego systemu drukowania Windows i może współpracować
z różnymi drukarkami zainstalowanymi w systemie. Fizyczną walidację wydruku,
geometrii, kalibracji, QR i preflightu wykonano na:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

Walidacja sprzętowa innych modeli drukarek Windows nie została jeszcze
przeprowadzona.

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
