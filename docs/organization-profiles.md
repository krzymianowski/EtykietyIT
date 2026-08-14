# Profile organizacji

Profil organizacji (`OrganizationProfile`) opisuje niezależną organizację,
oddział, tenant albo jednostkę wewnętrzną korzystającą z aplikacji. Nie jest tym
samym co profil etykiety (`LabelProfile`).

## Profil organizacji a profil etykiety

Profil organizacji przechowuje dane biznesowe i stan numeracji:

- nazwę profilu organizacji,
- nazwę firmy drukowaną na etykiecie,
- prefiks i liczbę cyfr Asset ID,
- następny numer Asset ID,
- identyfikator domyślnego profilu etykiety,
- nazwę domyślnej drukarki.

Profil etykiety opisuje wyłącznie format fizyczny: szerokość, wysokość, liczbę
kolumn i wierszy oraz linie cięcia. Profile użytkownika etykiet nadal znajdują
się w `profiles/user.<guid>.json`.

## Przechowywanie

Każda organizacja jest osobnym dokumentem JSON:

```text
organizations/organization.<guid>.json
```

Katalog `organizations` znajduje się pod katalogiem danych odpowiednim dla
trybu aplikacji:

- Standard: `%LOCALAPPDATA%\EtykietyIT\v3\organizations`,
- Portable: `<katalog EXE>\Data\v3\organizations`.

Globalny `settings.json` ma `schemaVersion` 2 i wskazuje tylko aktywną
organizację przez `activeOrganizationProfileId`.

## Globalna kalibracja drukarki

Kalibracja nie należy do organizacji. Offset X/Y opisuje zachowanie konkretnej
drukarki i pozostaje globalnie przypisany do jej nazwy w
`printer-calibrations.json`. Dzięki temu przełączenie organizacji nie duplikuje
ani nie zmienia fizycznej kalibracji tej samej drukarki.

## Migracja settings.json v1 do v2

Przy wykryciu `schemaVersion: 1` aplikacja:

1. odczytuje i waliduje wszystkie ustawienia v1,
2. zapisuje dokładną kopię starego pliku jako `settings.v1.backup.json`,
3. tworzy albo ponownie wykorzystuje zgodny profil organizacji,
4. zapisuje nową organizację atomowo w katalogu `organizations`,
5. zapisuje atomowo `settings.json` z `schemaVersion: 2`, wskazujący nową
   organizację.

Migracja zachowuje firmę, format Asset ID, numerację, domyślną drukarkę i profil
etykiety. Po udanej migracji kolejne uruchomienie odczytuje już format v2 i nie
tworzy następnej organizacji ani kopii. Jeżeli migracja nie zostanie zakończona,
stary `settings.json` pozostaje źródłem danych, a pliki utworzone przez nieudaną
próbę są wycofywane. Zgodny profil pozostały po przerwaniu procesu może zostać
bezpiecznie ponownie użyty.

Migracja nie modyfikuje kalibracji drukarek, profili etykiet ani historii JSONL.
