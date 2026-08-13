ETYKIETY IT v2.4
================

Ta wersja naprawia problem z linią środka ujawniony przez diagnostykę
na DYMO LabelWriter 450 i 550.

WYNIK DIAGNOSTYKI
-----------------
Dla profilu 89 x 41 mm sterownik raportuje po obróceniu do Landscape:

Bounds:
  88,90 x 41,40 mm

HardMargin:
  X = 5,84 mm
  Y = 1,02 mm

Graphics.VisibleClipBounds:
  81,53 x 38,86 mm

To oznacza, że Graphics X=0 na prawdziwym wydruku zaczyna się około
5,84 mm od fizycznej lewej krawędzi etykiety.

BŁĄD W v2.2 / v2.3
------------------
Kod odczytywał prawidłowe HardMarginX=5,84 mm, ale następnie
nadpisywał je PrintableArea.X=1,02 mm.

Przy Landscape PrintableArea zachowuje orientację/osiowanie, które
nie odpowiada bezpośrednio osi X renderowanego wydruku.

W efekcie korekta wynosiła tylko około 1 mm zamiast około 5,84 mm.

CO ZMIENIONO
------------
- na fizycznym wydruku używany jest bezpośrednio HardMarginX/HardMarginY,
- szerokość strony pochodzi z PageSettings.Bounds,
- dla 88,90 mm środek wypada w 44,45 mm,
- rzeczywisty obszar drukowalny pobierany jest z VisibleClipBounds,
- prawy/dolny margines jest wyliczany z:
    Bounds - HardMargin - VisibleClipBounds,
- podgląd nie jest przesuwany o HardMargin, dzięki czemu nadal pokazuje
  naturalny fizyczny układ etykiety,
- tryb Diagnostyka z v2.3 pozostaje dostępny.

TEST
----
Wybierz:
  89 x 41 mm — 2 szt. w poziomie
  liczba etykiet: 2

Linia cięcia na fizycznym wydruku powinna teraz wypaść około:
  44,45 mm od lewej krawędzi
  44,45 mm od prawej krawędzi

Jeśli pozostanie niewielkie przesunięcie rzędu np. 0,5-1 mm,
kolejnym krokiem będzie dodanie zapamiętywanej kalibracji X/Y.

DANE
----
v2.4 używa tych samych:
%LOCALAPPDATA%\EtykietyIT\settings.json
%LOCALAPPDATA%\EtykietyIT\history_v2.csv

Historia i numeracja zostają zachowane.
