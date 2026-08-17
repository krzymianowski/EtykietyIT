[English](README.md) | Polski

# Etykiety IT

Etykiety IT to aplikacja Windows do tworzenia i drukowania etykiet
inwentarzowych dla zasobów IT, wyposażona w kody QR, profile organizacji,
kalibrację drukarek i historię wydruków.

> Aktualne wydanie: `3.0.0`

## Funkcje

- profile organizacji z niezależną numeracją Asset ID,
- konfigurowalny prefiks Asset ID i liczba cyfr,
- wbudowane i własne profile etykiet,
- obsługa układów wieloetykietowych,
- podgląd przed wydrukiem,
- osobna kalibracja X/Y dla każdej drukarki,
- opcjonalny kod QR zawierający dokładnie sformatowany Asset ID,
- dynamiczne dopasowanie wielkości QR z minimum 4 punktów na moduł,
- preflight drukowalności blokujący nieczytelne lub niemożliwe kombinacje
  drukarki, profilu etykiety i treści,
- trwała historia wydruków z wyszukiwaniem i filtrowaniem,
- eksport widocznych rekordów historii do CSV i XLSX,
- tryb Standard i tryb Portable,
- obsługa High DPI w trybie PerMonitorV2.

## Zweryfikowane drukarki

Fizyczne testy wydruku, geometrii, kalibracji, QR i preflightu drukowalności
wykonano na:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

Etykiety IT korzysta ze standardowego systemu drukowania Windows i może
współpracować z innymi drukarkami zainstalowanymi w systemie. Inne modele nie
zostały objęte pełną walidacją sprzętową wersji 3.0.0.

Szczegółowe wyniki testów sprzętowych znajdują się w
[`docs/printing-validation.md`](docs/printing-validation.md).
Projekt QR i reguły walidacji opisują
[`docs/qr.md`](docs/qr.md) oraz
[`docs/printability-preflight.md`](docs/printability-preflight.md).

## Pobieranie

Aktualne wydanie można pobrać z [GitHub Releases](https://github.com/krzymianowski/EtykietyIT/releases).

Dostępne są dwa archiwa dla Windows x64:

- **Standard** — zapisuje dane użytkownika w profilu bieżącego użytkownika
  Windows,
- **Portable** — zapisuje dane użytkownika obok aplikacji.

Oba warianty zawierają tę samą aplikację self-contained. Archiwum Portable
różni się wyłącznie obecnością pliku `portable.mode`.

## Standard a Portable

W **trybie Standard** dane użytkownika są przechowywane w:

```text
%LOCALAPPDATA%\EtykietyIT\v3
```

W **trybie Portable** dane użytkownika są przechowywane w:

```text
<katalog aplikacji>\Data\v3
```

Tryb Portable można włączyć parametrem `--portable` albo pustym plikiem
`portable.mode` umieszczonym obok pliku wykonywalnego. Katalog aplikacji musi
być zapisywalny. Jeżeli zapis nie jest możliwy, aplikacja zgłasza błąd i nie
przełącza się automatycznie na tryb Standard.

Więcej informacji znajduje się w
[`docs/portable-mode.md`](docs/portable-mode.md).

## Wymagania

- 64-bitowy Windows obsługiwany przez .NET 10,
- system drukowania Windows,
- zainstalowany sterownik wybranej drukarki.

Archiwa wydania są self-contained. Uruchomienie Etykiety IT nie wymaga:

- .NET Desktop Runtime,
- .NET SDK,
- Microsoft Excel do eksportu XLSX.

## Brak podpisu cyfrowego

Binaria Etykiety IT 3.0.0 nie są podpisane cyfrowo. Microsoft Defender
SmartScreen może przy pierwszym uruchomieniu wyświetlić ostrzeżenie.
Opublikowane archiwa wydania zawierają sumy SHA-256 umożliwiające sprawdzenie
ich integralności.

## Budowanie ze źródeł

Budowanie wymaga systemu Windows i wersji .NET SDK wskazanej w pliku
[`global.json`](global.json). W katalogu głównym repozytorium uruchom:

```powershell
dotnet restore
dotnet build
dotnet test
```

Projekt wykorzystuje Windows Forms i `System.Drawing.Printing`. Pełny proces
pakowania wydania opisuje [`docs/release.md`](docs/release.md), a wskazówki
deweloperskie znajdują się w
[`docs/development.md`](docs/development.md).

## Struktura projektu

```text
EtykietyIT/             aplikacja Windows Forms
  Bootstrap/            ręczne składanie zależności
  Export/               eksport historii CSV/XLSX
  Forms/                formularze aplikacji
  Models/               modele danych
  Persistence/          ścieżki aplikacji i zapis JSON
  Printing/             fizycznie zweryfikowany silnik drukowania
  Resources/Profiles/   wbudowane profile etykiet
  Services/             serwisy aplikacyjne
EtykietyIT.Tests/       testy automatyczne
docs/                   dokumentacja techniczna i utrzymaniowa
legacy/                 referencyjna implementacja PowerShell 2.4
```

## Linki repozytorium

- Repozytorium: [github.com/krzymianowski/EtykietyIT](https://github.com/krzymianowski/EtykietyIT)
- Wydania: [github.com/krzymianowski/EtykietyIT/releases](https://github.com/krzymianowski/EtykietyIT/releases)
- Zgłaszanie problemów: [github.com/krzymianowski/EtykietyIT/issues](https://github.com/krzymianowski/EtykietyIT/issues)

Błędy i propozycje zmian można zgłaszać przez
[GitHub Issues](https://github.com/krzymianowski/EtykietyIT/issues).

## Współtworzenie i bezpieczeństwo

- Zasady współtworzenia projektu: [`CONTRIBUTING.md`](CONTRIBUTING.md)
- Polityka bezpieczeństwa i prywatne zgłaszanie podatności: [`SECURITY.md`](SECURITY.md)

## Licencja

Etykiety IT jest udostępniany na licencji MIT. Zobacz plik
[`LICENSE`](LICENSE). Informacje o użytych bibliotekach znajdują się w
[`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md).
