# QR z Asset ID

## Payload i kodowanie

Kod QR zawiera dokładnie ten sam sformatowany Asset ID, który jest drukowany
tekstowo, np. `IT-000123`. Payload nie zawiera adresu URL, spacji, końca linii,
opisu ani innych danych.

Do kodowania używany jest `Net.Codecrete.QrCodeGenerator` 3.1.0 z poziomem
korekcji `QrCode.Ecc.Medium`. Rozmiar macierzy jest zawsze pobierany z
`QrCode.Size`; implementacja nie zakłada na stałe QR Version 1.

## Geometria

- preferowany maksymalny footprint QR: 15,0 × 15,0 mm,
- quiet zone: dokładnie 4 moduły z każdej strony,
- QR znajduje się po lewej stronie białej części etykiety,
- odstęp od tekstu: 1,5 mm,
- dolny czarny pasek zachowuje dotychczasową pozycję, wysokość i szerokość,
- dla profilu 1-up QR nie jest powiększany.

Renderer wyznacza dostępny kwadrat jako minimum z 15 mm, dostępnej szerokości
strefy po lewej oraz wysokości białej części nad paskiem firmy. Mniejsza komórka
może więc otrzymać mniejszy QR bez zmiany geometrii profilu, paska albo linii
cięcia.

Moduły są rysowane bezpośrednio przez `Graphics.FillRectangle`. Nie jest używana
bitmapa ani interpolacja obrazu. Zmiany `SmoothingMode` są ograniczone do
lokalnego `GraphicsState` i przywracane po narysowaniu QR.

## Fizyczny wydruk i całkowite punkty modułu

Dla fizycznego wydruku rozmiar modułu jest wyrównywany do całkowitej liczby
punktów wynikającej z rzeczywistego `Graphics.DpiX` drukarki:

```text
dotMm = 25,4 / Graphics.DpiX
totalModules = QrCode.Size + 8
maxFootprintMm = min(15,0, dostępna szerokość, dostępna wysokość)
dotsPerModule = floor(maxFootprintMm / totalModules / dotMm)
moduleSizeMm = dotsPerModule × dotMm
actualFootprintMm = totalModules × moduleSizeMm
```

Wymagane minimum wynosi 4 dots/module. Jeżeli nie można go osiągnąć, zadanie
jest odrzucane przed `PrintDocument.Print()`. W pozostałych przypadkach używana
jest największa całkowita liczba punktów na moduł mieszcząca się w dostępnej
strefie, maksymalnie 15 mm. QR nie jest rozciągany do ułamkowej liczby punktów.
Rzeczywisty footprint jest wyśrodkowany w dostępnej strefie.

Dla `IT-000123` powstaje QR Version 1: 21 modułów oraz 8 modułów quiet zone,
czyli 29 modułów łącznie. Na DYMO LabelWriter 450/550 o rozdzielczości 300 DPI:

```text
dotsPerModule = 6
moduleSizeMm = 6 × 25,4 / 300 = 0,5080 mm
actualFootprintMm = 29 × 0,5080 = 14,732 mm
```

Dla tej samej macierzy i 300 DPI dynamiczne warianty wynoszą:

| Dostępne maksimum | Punkty/moduł | Rzeczywisty footprint |
|---:|---:|---:|
| 15 mm | 6 | 14,73 mm |
| 13 mm | 5 | 12,28 mm |
| 11 mm | 4 | 9,82 mm |
| 9 mm | 3 | odrzucenie, poniżej minimum |

## Preview

Preview wykorzystuje dostępny footprint, maksymalnie 15 mm, i rysuje kwadratowe
moduły wektorowo. Jeśli sterownik wybranej drukarki udostępnia DPI pomiarowe,
podgląd używa odpowiadającej mu całkowitej liczby punktów na moduł, aby rozmiar
był zbliżony do wydruku. Jeżeli nie jest to możliwe, używany jest dostępny
footprint bez walidacji ekranowego DPI. `Graphics.DpiX` monitora nie decyduje o
poprawności Preview.

## Walidacja sprzętowa

QR został fizycznie zweryfikowany na drukarkach o rozdzielczości 300 DPI:

- DYMO LabelWriter 450: 89 × 41 mm / 2-up / QR OFF i QR ON oraz 2 × 2 / QR ON,
- DYMO LabelWriter 550: 89 × 41 mm / 1-up i 2-up / QR OFF i QR ON oraz
  2 × 2 / QR ON.

Na obu urządzeniach kody QR z fizycznego wydruku zostały poprawnie
zeskanowane. Asset ID, geometria, pasek firmy i linie cięcia pozostały
prawidłowe. Preflight poprawnie blokuje nieczytelny układ 89 × 41 mm / 3 × 3.
Szczegółowa macierz znajduje się w `docs/printing-validation.md`.
