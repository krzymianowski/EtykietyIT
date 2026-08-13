# Etykiety IT - instrukcje dla Codexa

## Projekt
Etykiety IT to aplikacja Windows do drukowania etykiet inwentarzowych.

Technologie:
- C#
- .NET 10
- Windows Forms
- System.Drawing / PrintDocument

## Najważniejsza zasada
Plik `legacy/EtykietyIT_v2.4.ps1` jest referencyjną, działającą implementacją
silnika drukowania.

Nie modyfikuj plików w katalogu `legacy/`.

Podczas migracji silnika drukowania do C# zachowaj dokładnie zachowanie v2.4,
w szczególności:
- obsługę `HardMarginX` i `HardMarginY`,
- `VisibleClipBounds`,
- `PageSettings.Bounds`,
- orientację Landscape,
- kompensację fizycznych marginesów DYMO,
- podział etykiety 89 x 41 mm na dwie równe części.

Nie upraszczaj ani nie refaktoryzuj mechanizmu drukowania, dopóki wersja C#
nie zostanie fizycznie przetestowana na drukarkach DYMO LabelWriter 450 i 550.

## Sposób pracy
- Wprowadzaj małe, kontrolowane zmiany.
- Po zmianach uruchamiaj `dotnet build`.
- Nie dodawaj nowych funkcji bez wyraźnego polecenia.
- Nie zmieniaj publicznego zachowania aplikacji bez uzasadnienia.
- Preferuj czytelny, prosty kod.
- Używaj nullable reference types.
- Traktuj warningi kompilatora jako problemy do rozwiązania.

## Git
- Nie wykonuj commitów bez wyraźnego polecenia.
- Nie zmieniaj historii Git.
- Nie usuwaj istniejących plików bez potrzeby.