# Tryb Portable

## Wykrywanie trybu

Kolejność wykrywania jest stała:

1. argument `--portable`, bez rozróżniania wielkości liter,
2. plik `portable.mode` obok pliku wykonywalnego,
3. tryb Standard, jeżeli żaden z powyższych warunków nie jest spełniony.

Argument `--portable` ma najwyższy priorytet.

## Lokalizacja danych

Tryb Standard:

```text
%LOCALAPPDATA%\EtykietyIT\v3
```

Tryb Portable:

```text
<katalog EXE>\Data\v3
```

`AppDataPaths` wyłącznie oblicza ścieżki i nie tworzy katalogów. Katalogi
tworzy `AppBootstrapper`.

## Sprawdzenie zapisu

Po utworzeniu katalogów bootstrap tworzy w katalogu danych tymczasowy plik,
zapisuje do niego bajt i usuwa go. Nieudana próba zapisu przerywa bootstrap.

W trybie Portable nie wolno automatycznie przechodzić do trybu Standard.
Warstwa UI zostanie później odpowiedzialna za pokazanie użytkownikowi czytelnej
informacji o niezapisywalnym katalogu.
