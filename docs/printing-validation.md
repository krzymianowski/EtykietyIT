# Walidacja silnika drukowania

Renderer `Printing/LabelPrintJob.cs` i jego geometria są traktowane jako stabilne.
Zmiany dotyczące `PageSettings.Bounds`, `HardMarginX`, `HardMarginY`,
`VisibleClipBounds`, transformacji albo podziału strony wymagają wyraźnego
polecenia i ponownej fizycznej weryfikacji.

## Potwierdzone testy fizyczne

### DYMO LabelWriter 450

- Offset X = 0,0 mm
- Offset Y = 0,0 mm
- środek poprawny

### DYMO LabelWriter 550

- Offset X = -0,4 mm
- Offset Y = 0,0 mm
- środek poprawny

## Preview

- symetryczny
- bez kompensacji `HardMargin`
- bez `PrinterCalibration`

Tryb renderowania jest przekazywany jawnie przez `LabelRenderMode`. Preview nie
może być wykrywany na podstawie nazwy typu `PrintController`.

## Znaczenie udanego wywołania Print

Bezbłędne zakończenie `PrintDocument.Print()` oznacza przekazanie zadania do
systemu drukowania. Nie stanowi potwierdzenia, że etykieta została fizycznie
wydrukowana.
