# Etykiety IT 3.0.0

Etykiety IT 3.0.0 is the first stable release of a Windows application for
creating and printing IT asset labels.

## Highlights

- organization profiles with independent Asset ID numbering,
- configurable Asset ID prefix and number of digits,
- built-in and user-defined label profiles,
- multi-label layouts,
- print preview,
- per-printer calibration,
- optional QR code containing the exact formatted Asset ID,
- dynamic QR sizing with a minimum of 4 dots per module,
- printability preflight that blocks unreadable or impossible layouts,
- persistent print history with search and filtering,
- CSV and XLSX export without requiring Microsoft Excel,
- Standard mode and Portable mode,
- High DPI support using PerMonitorV2.

## Verified printers

Physical validation was completed on:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

The tests covered printing with and without QR codes, 2-up and 2×2 layouts,
QR scanning, printer calibration, and printability preflight. Etykiety IT uses
the standard Windows printing system, so other printers may work, but they were
not included in the full v3.0.0 hardware validation.

## Downloads

### Standard

`EtykietyIT-3.0.0-win-x64.zip`

User data is stored in:

```text
%LOCALAPPDATA%\EtykietyIT\v3
```

### Portable

`EtykietyIT-3.0.0-win-x64-portable.zip`

User data is stored in `Data\v3` inside the application directory. The
Portable directory must be writable.

## Requirements

- Windows x64,
- the Windows printing system,
- an installed driver for the selected printer.

The packages are self-contained. Installing .NET Desktop Runtime, .NET SDK,
or Microsoft Excel is not required.

### Language

Etykiety IT 3.0.0 currently provides a Polish-language user interface.
English UI support is planned for version 3.1.0.

## Unsigned binaries

The Etykiety IT 3.0.0 binaries are not digitally signed. Microsoft Defender
SmartScreen may display a warning when running the application for the first
time. Use the published `SHA256SUMS.txt` file to verify the integrity of the
downloaded archives.

## SHA-256

Standard:

```text
9c294f00add38300ab80bf596e288674129491a849dc8107c00460f867813013  EtykietyIT-3.0.0-win-x64.zip
```

Portable:

```text
d64ebe0087d71b22a84020c6eec27156b7e3d91d1c62afc0660f8c33c159ec50  EtykietyIT-3.0.0-win-x64-portable.zip
```

## Validation

This release was validated with:

- 117 automated tests,
- physical tests on DYMO LabelWriter 450 and 550,
- Standard and Portable mode tests,
- tests on three computers, including a clean Windows installation.

## License

Etykiety IT is available under the MIT License.

---

<details>
<summary>Polski</summary>

Etykiety IT 3.0.0 to pierwsze stabilne wydanie aplikacji Windows do tworzenia
i drukowania etykiet inwentarzowych dla zasobów IT.

## Najważniejsze funkcje

- profile organizacji z niezależną numeracją Asset ID,
- konfigurowalny prefiks Asset ID i liczba cyfr,
- wbudowane i własne profile etykiet,
- obsługa układów wieloetykietowych,
- podgląd przed wydrukiem,
- osobna kalibracja każdej drukarki,
- opcjonalny kod QR zawierający dokładnie sformatowany Asset ID,
- dynamiczne dopasowanie wielkości QR z minimum 4 punktów na moduł,
- preflight drukowalności blokujący nieczytelne lub niemożliwe layouty,
- trwała historia wydruków z wyszukiwaniem i filtrowaniem,
- eksport CSV i XLSX bez wymagania Microsoft Excel,
- tryb Standard i tryb Portable,
- obsługa High DPI w trybie PerMonitorV2.

## Zweryfikowane drukarki

Fizyczną walidację wykonano na:

- DYMO LabelWriter 450,
- DYMO LabelWriter 550.

Testy obejmowały wydruk bez QR i z QR, układy 2-up i 2×2, skanowanie kodów
QR, kalibrację drukarek oraz preflight drukowalności. Etykiety IT korzysta ze
standardowego systemu drukowania Windows, dlatego inne drukarki mogą działać,
ale nie zostały objęte pełną walidacją sprzętową wersji 3.0.0.

## Pobieranie

### Standard

`EtykietyIT-3.0.0-win-x64.zip`

Dane użytkownika są przechowywane w:

```text
%LOCALAPPDATA%\EtykietyIT\v3
```

### Portable

`EtykietyIT-3.0.0-win-x64-portable.zip`

Dane użytkownika są przechowywane w `Data\v3` wewnątrz katalogu aplikacji.
Katalog wersji Portable musi być zapisywalny.

## Wymagania

- Windows x64,
- system drukowania Windows,
- zainstalowany sterownik wybranej drukarki.

Pakiety są self-contained. Instalowanie .NET Desktop Runtime, .NET SDK ani
Microsoft Excel nie jest wymagane.

### Język

Etykiety IT 3.0.0 posiada obecnie wyłącznie polski interfejs użytkownika.
Obsługa języka angielskiego jest planowana w wersji 3.1.0.

## Brak podpisu cyfrowego

Binaria Etykiety IT 3.0.0 nie są podpisane cyfrowo. Microsoft Defender
SmartScreen może przy pierwszym uruchomieniu wyświetlić ostrzeżenie. Do
sprawdzenia integralności pobranych archiwów służy opublikowany plik
`SHA256SUMS.txt`.

## SHA-256

Standard:

```text
9c294f00add38300ab80bf596e288674129491a849dc8107c00460f867813013  EtykietyIT-3.0.0-win-x64.zip
```

Portable:

```text
d64ebe0087d71b22a84020c6eec27156b7e3d91d1c62afc0660f8c33c159ec50  EtykietyIT-3.0.0-win-x64-portable.zip
```

## Walidacja

Wydanie zweryfikowano przez:

- 117 testów automatycznych,
- testy fizyczne na DYMO LabelWriter 450 i 550,
- testy trybu Standard i Portable,
- testy na trzech komputerach, w tym na czystej instalacji Windows.

## Licencja

Etykiety IT jest udostępniany na licencji MIT.

</details>
