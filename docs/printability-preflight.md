# Preflight drukowalności etykiet

## Dlaczego profil nie wystarcza

Profil etykiety opisuje papier i podział na komórki, ale sam w sobie nie jest
trwale „drukowalny” albo „niedrukowalny”. Wynik zależy równocześnie od:

- rzeczywistego rozmiaru strony zwróconego przez sterownik drukarki,
- liczby kolumn i wierszy,
- sprzętowych marginesów wybranej drukarki,
- nazwy firmy,
- prefiksu, liczby cyfr i najdłuższego Asset ID w bieżącym zadaniu,
- włączenia QR, rozmiaru jego macierzy i DPI drukarki.

Dlatego zapis profilu nie jest blokowany. Formularz profilu pokazuje
informacyjnie deklarowany rozmiar pojedynczej komórki, natomiast autorytatywny
preflight jest wykonywany dla bieżących danych przed podglądem i drukiem.

## Źródła geometrii

`LabelPrintabilityValidator` konfiguruje `PrinterSettings`, `PaperSelection` i
`PageSettings` tak samo jak `LabelPrintJob`. Rozmiar strony pochodzi z runtime
`PageSettings.Bounds`, a rozmiar komórki jest liczony jako:

```text
cellWidth = runtimePageWidth / Columns
cellHeight = runtimePageHeight / Rows
```

Po ustawieniu `Graphics.PageUnit = Millimeter` validator odczytuje
`VisibleClipBounds`. Bezpieczne krawędzie wylicza z `HardMarginX/Y` i przeciwnej
krawędzi wynikającej z `VisibleClipBounds`, zgodnie z istniejącym rendererem.
Nie używa `PrintableArea.X/Y`, nie wykonuje `TranslateTransform` i nie zmienia
mechanizmu kalibracji. Measurement graphics pochodzi z wybranej drukarki, więc
DPI monitora nie wpływa na wynik.

## Zakres walidacji

Wynik zawiera listę problemów oraz metryki użyte do decyzji:

- faktyczny rozmiar strony i pojedynczej komórki,
- najmniejszy dostępny obszar treści i białą strefę,
- wysokość paska firmy,
- najmniejsze wyliczone fonty tytułu, Asset ID i firmy,
- dla QR: `QrCode.Size`, liczbę modułów z quiet zone, `dotsPerModule` i
  rzeczywisty footprint.

Validator wykrywa między innymi nieprawidłowy rozmiar strony lub komórki, brak
miejsca po paddingach, brak białej strefy albo paska firmy, wyjście prostokątów
rzeczywiście rysowanego tekstu poza białą część, kolizję tytułu z Asset ID,
kolizję Asset ID lub QR z paskiem oraz zbyt wąską strefę tekstową obok QR.

Prostokąty layoutu przekazywane do `Graphics.DrawString` mogą celowo minimalnie
na siebie zachodzić, ponieważ tekst jest w nich wyśrodkowany. Samo przecięcie
tych stref nie jest więc błędem. Validator używa `MeasureString` i ocenia
wyśrodkowane granice faktycznie rysowanych tekstów. Wspólna czysta geometria
`LabelQrLayoutGeometry` dostarcza identyczne prostokąty QR, tytułu, Asset ID i
paska do gałęzi QR renderera oraz do validatora.

Najdłuższy rzeczywisty Asset ID jest wybierany osobno dla każdego zajętego
miejsca na stronie. Pomiar fontu odwzorowuje algorytm renderera: Arial,
odpowiedni styl, `Graphics.MeasureString`, krok 0,5 pt i tolerancję wysokości
1,15. Stara ścieżka renderowania bez QR nie została przy tym refaktoryzowana.

## Czytelność tekstu

Przyjęte twarde minima to:

| Element | Minimum | Uzasadnienie |
|---|---:|---|
| `Nr inwentarzowy` | 5,0 pt | zaakceptowany układ 2 × 2 używa 5,0 pt |
| Asset ID | 8,0 pt | dolna granica konfiguracji renderera; zaakceptowany układ 2 × 2 używa 9,10 pt |
| Nazwa firmy | 4,5 pt | zaakceptowany układ 2 × 2 używa 4,5 pt |

Font poniżej minimum albo tekst, który nie mieści się nawet przy minimalnym
foncie renderera, jest błędem. Wartość w przedziale do 0,5 pt ponad minimum jest
ostrzeżeniem i sama nie blokuje zadania.

Dla testowego `IT-000123` i nazwy `Dolnośląskie Młyny S.A.` pomiar przy stronie
89 × 41 mm, bezpiecznych krawędziach 1,5 mm i 300 DPI daje:

| Układ | QR | Tytuł | Asset ID | Firma |
|---|---|---:|---:|---:|
| 1 × 1 | wył. | 6,84 pt | 19,00 pt | 6,08 pt |
| 2 × 1 | wył. | 6,84 pt | 19,00 pt | 6,08 pt |
| 1 × 1 | wł. | 6,84 pt | 19,00 pt | 6,08 pt |
| 2 × 1 | wł. | 6,84 pt | 14,00 pt | 6,08 pt |
| 2 × 2 | wył. | 5,00 pt | 9,10 pt | 4,50 pt |
| 2 × 2 | wł. | 5,00 pt | 9,10 pt | 4,50 pt |

Układ 2 × 2 z QR jest przypadkiem golden potwierdzonym fizycznie na DYMO
LabelWriter 450 i 550. Przy `safeEdge=1,5 mm` layout pierwszej komórki ma 41,5 ×
18,2 mm, strefa QR 12,4 mm, a rzeczywisty QR 12,277 mm przy 5 dots/module.
Przydzielone prostokąty tytułu i Asset ID przecinają się o 0,088 mm, ale
zmierzone teksty mają około 1,78 mm pionowego odstępu i nie kolidują.

## QR

Preflight używa wspólnego obliczenia `LabelQrRenderer`. Preferowany footprint
wynosi maksymalnie 15 mm, quiet zone ma 4 moduły z każdej strony, a fizyczny
moduł musi mieć co najmniej 4 całkowite punkty drukarki. Mniejsze komórki mogą
użyć 5 albo 4 dots/module; zadanie jest odrzucane dopiero wtedy, gdy nie mieści
się minimum 4 dots/module.

## Error i Warning

- `Error` oznacza przewidywalną kolizję, brak miejsca albo zejście poniżej
  przyjętej jakości. Blokuje podgląd i druk.
- `Warning` informuje o wartości bliskiej minimum, ale nie blokuje zadania.

Ten sam fizyczny preflight wybranej drukarki działa przed
`PrintPreviewDialog.ShowDialog()` i przed `PrintDocument.Print()`. Preview nie
jest oceniany na podstawie DPI ekranu. Przy błędzie nie jest otwierany dialog
podglądu, nie jest wywoływany `Print()`, nie powstaje historia i nie zmienia się
`NextAssetNumber`.

## Przykład 89 × 41 mm / 3 × 3

Dla runtime 89 × 41 mm i bezpiecznych krawędzi 1,5 mm validator wylicza komórkę
około 29,67 × 13,67 mm, minimalny obszar treści 27,13 × 11,37 mm i białą część
wysokości 5,57 mm. Zmierzony tekst Asset ID wchodzi wtedy w pasek firmy.
Wariant z QR dodatkowo nie osiąga minimum 4 dots/module przy 300 DPI. Profil
może nadal istnieć i zostać użyty na większym papierze, ale ta konkretna
kombinacja drukarki, strony i treści jest blokowana.

## Status walidacji sprzętowej

Preflight został praktycznie zweryfikowany na DYMO LabelWriter 450 i 550.
Przepuszcza czytelne układy 1-up, 2-up i 2 × 2 zgodnie z macierzą testów, a na
obu drukarkach poprawnie blokuje nieczytelną kombinację 89 × 41 mm / 3 × 3.
Nie wprowadza arbitralnego limitu liczby wierszy ani kolumn; decyzja nadal
wynika z bieżącej drukarki, rozmiaru strony, treści i QR.
