# Walidacja silnika drukowania

Renderer `Printing/LabelPrintJob.cs` i jego geometria są traktowane jako stabilne.
Zmiany dotyczące `PageSettings.Bounds`, `HardMarginX`, `HardMarginY`,
`VisibleClipBounds`, transformacji albo podziału strony wymagają wyraźnego
polecenia i ponownej fizycznej weryfikacji.

## Potwierdzone testy fizyczne

### DYMO LabelWriter 450

- Rozdzielczość głowicy = 300 DPI
- Offset X = 0,0 mm
- Offset Y = 0,0 mm
- środek poprawny
- profil 89 × 41 mm 2-up, QR OFF: PASS
- profil 89 × 41 mm 2-up, QR ON: PASS
- profil 89 × 41 mm 2 × 2 / 4 małe etykiety, QR ON: PASS
- skan QR z fizycznego wydruku: PASS
- geometria, pasek firmy i linie cięcia: PASS
- profil 89 × 41 mm 3 × 3: EXPECTED FAIL — poprawnie zablokowany przez preflight

### DYMO LabelWriter 550

- Rozdzielczość głowicy = 300 DPI
- Offset X = -0,4 mm
- Offset Y = 0,0 mm
- środek poprawny
- profil 89 × 41 mm 1-up, QR OFF: PASS
- profil 89 × 41 mm 2-up, QR OFF: PASS
- profil 89 × 41 mm 1-up, QR ON: PASS
- profil 89 × 41 mm 2-up, QR ON: PASS
- profil 89 × 41 mm 2 × 2 / 4 małe etykiety, QR ON: PASS
- profil 89 × 41 mm 2 × 2 / 4 małe etykiety, skan QR: PASS
- skan QR z Preview: PASS
- skan QR z fizycznego wydruku: PASS
- payload QR zgodny z Asset ID: PASS
- profil 89 × 41 mm 3 × 3: EXPECTED FAIL — poprawnie zablokowany przez preflight

### Macierz walidacji sprzętowej

| Drukarka | Scenariusz | Wynik |
|---|---|---|
| DYMO LabelWriter 450 | 89 × 41 mm / 2-up / QR OFF | PASS |
| DYMO LabelWriter 450 | 89 × 41 mm / 2-up / QR ON | PASS |
| DYMO LabelWriter 450 | 89 × 41 mm / 2 × 2 / QR ON | PASS |
| DYMO LabelWriter 450 | skan QR | PASS |
| DYMO LabelWriter 450 | 89 × 41 mm / 3 × 3 | EXPECTED FAIL — preflight blokuje |
| DYMO LabelWriter 550 | 89 × 41 mm / 1-up / QR OFF | PASS |
| DYMO LabelWriter 550 | 89 × 41 mm / 2-up / QR OFF | PASS |
| DYMO LabelWriter 550 | 89 × 41 mm / 1-up / QR ON | PASS |
| DYMO LabelWriter 550 | 89 × 41 mm / 2-up / QR ON | PASS |
| DYMO LabelWriter 550 | 89 × 41 mm / 2 × 2 / QR ON | PASS |
| DYMO LabelWriter 550 | skan QR | PASS |
| DYMO LabelWriter 550 | 89 × 41 mm / 3 × 3 | EXPECTED FAIL — preflight blokuje |

### Oczekiwane odrzucenie 89 × 41 mm / 3 × 3

- preflight: EXPECTED FAIL
- QR OFF: Asset ID kolidowałby z paskiem firmy; komórka nie zapewnia
  wystarczającej przestrzeni na czytelny layout
- QR ON: jak wyżej, a dodatkowo QR nie osiąga minimum 4 dots/module

Jest to poprawnie odrzucona kombinacja drukarki, profilu i aktualnej treści, a
nie błąd programu. Profil 3 × 3 nie jest arbitralnie zabroniony i może być
drukowalny na większym papierze lub dla innej geometrii.

## Preview

- symetryczny
- bez kompensacji `HardMargin`
- bez `PrinterCalibration`

Tryb renderowania jest przekazywany jawnie przez `LabelRenderMode`. Preview nie
może być wykrywany na podstawie nazwy typu `PrintController`.

## Status walidacji QR

Układ bez QR pozostaje fizycznie zweryfikowany zgodnie z powyższymi wynikami.
Ścieżka QR i preflight zostały fizycznie zweryfikowane na DYMO LabelWriter 450
i 550. W układzie 2 × 2 wszystkie cztery kody QR zostały poprawnie zeskanowane,
Asset ID pozostały czytelne, a pasek firmy i linie cięcia zachowały prawidłową
geometrię. Na obu drukarkach preflight poprawnie blokuje nieczytelną kombinację
89 × 41 mm / 3 × 3.

## Znaczenie udanego wywołania Print

Bezbłędne zakończenie `PrintDocument.Print()` oznacza przekazanie zadania do
systemu drukowania. Nie stanowi potwierdzenia, że etykieta została fizycznie
wydrukowana.
