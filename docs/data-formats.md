# Formaty danych Etykiety IT v3

## Wspólne reguły JSON

- kodowanie UTF-8,
- nazwy właściwości `camelCase`,
- czytelne formatowanie `WriteIndented = true`,
- jawne pole `schemaVersion` dla wersjonowanych dokumentów,
- zapis atomowy: plik tymczasowy, deserializacja i walidacja, następnie
  `File.Replace` albo `File.Move`.

## Ustawienia

Plik `settings.json` zawiera jeden `ApplicationSettings`.

```json
{
  "schemaVersion": 1,
  "companyName": "Dolnośląskie Młyny S.A.",
  "assetId": {
    "prefix": "IT-",
    "digits": 6
  },
  "defaultPrinterName": "DYMO LabelWriter 550",
  "defaultProfileId": "builtin.89x41.2up",
  "nextAssetNumber": 11
}
```

## Kalibracje drukarek

Docelowy plik `printer-calibrations.json` będzie zawierał kolekcję wpisów
`PrinterCalibrationEntry`. Brak wpisu dla drukarki oznacza offset 0,0 / 0,0.

Nazwa drukarki jest kluczem logicznym porównywanym bez rozróżniania wielkości
liter. Nie należy automatycznie przypisywać kalibracji na podstawie samego modelu
drukarki.

## Profile etykiet

Każdy plik JSON zawiera dokładnie jeden `LabelProfile`.

Profile wbudowane znajdują się jako osobne pliki w repozytorium:

```text
EtykietyIT/Resources/Profiles/builtin.89x41.2up.json
EtykietyIT/Resources/Profiles/builtin.89x41.1up.json
...
```

W przyszłości mogą zostać osadzone jako `EmbeddedResource`.

Profile użytkownika znajdują się w katalogu danych:

```text
profiles/user.<guid>.json
```

Przykład pojedynczego pliku profilu:

```json
{
  "schemaVersion": 1,
  "id": "user.9675d94a-7103-482f-b18e-9b29ca64f646",
  "name": "Mój profil 89 × 41",
  "widthMm": 89.0,
  "heightMm": 41.0,
  "columns": 2,
  "rows": 1,
  "drawCutLines": true
}
```

## Historia wydruków

Historia jest przechowywana w `history/print-history.jsonl`. Każda linia jest
niezależnym dokumentem `PrintHistoryEntry` i zawiera co najmniej:

- `Guid id`,
- `DateTimeOffset timestampUtc` zapisany w UTC,
- `applicationVersion`,
- pełny snapshot parametrów przekazanego zadania.

Wpis jest tworzony wyłącznie po bezbłędnym zakończeniu
`PrintDocument.Print()`. Oznacza przekazanie zadania do systemu drukowania,
a nie potwierdzenie fizycznego wydruku.

Snapshot zachowuje m.in. drukarkę, kalibrację, firmę, format Asset ID, zakres,
profil wraz z geometrią oraz liczbę małych i fizycznych etykiet.
