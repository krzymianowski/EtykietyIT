# High DPI w aplikacji WinForms

## Konfiguracja aplikacji

Projekt ustawia `ApplicationHighDpiMode` na `PerMonitorV2`. Konfigurację
WinForms nadal wykonuje wyłącznie `ApplicationConfiguration.Initialize()` w
`Program.cs`. Kod aplikacji nie wywołuje ręcznie `Application.SetHighDpiMode`
i nie przelicza współrzędnych przez `DeviceDpi`.

Wszystkie formularze używają:

- `AutoScaleMode = AutoScaleMode.Dpi`,
- `AutoScaleDimensions = 96F, 96F` jako bazowej rozdzielczości projektowej,
- fontu Segoe UI dla standardowych kontrolek Windows.

Font Consolas pozostaje używany tylko tam, gdzie stała szerokość znaków
poprawia czytelność numerów Asset ID, szczegółów historii i dokumentów
tekstowych.

## Zasady układu formularzy

- Sekcje i wiersze zawierające tekst mają wysokość `AutoSize`.
- Główna przestrzeń robocza używa `Percent` oraz `Dock = Fill`.
- Paski akcji są w osobnych wierszach `AutoSize`.
- Przyciski o znanym tekście mają stały rozmiar projektowy w bazie 96 DPI;
  skaluje je mechanizm `AutoScaleMode.Dpi`. Kontenery pasków akcji mają
  wysokość `AutoSize` wynikającą z przycisków.
- Pola edycyjne rozszerzają się przez `Dock = Fill`.
- Etykiety pól mają `AutoSize` i nie korzystają ze stałej wysokości.
- Paski filtrów i przycisków mogą zawijać zawartość, jeśli zabraknie miejsca.
- Formularze nie mają sztywnego `MaximumSize`.
- `MinimumSize` jest stosowane tylko dla formularzy, w których zbyt małe okno
  uniemożliwiłoby używanie tabeli, panelu szczegółów lub pól edycyjnych.
- Układ nie jest skalowany ręcznie i nie zawiera poprawek zależnych od
  konkretnej wartości DPI.

## Zakres skalowania

Układy są przeznaczone do pracy przy skalowaniu Windows:

- 100%,
- 125%,
- 150%,
- 200%.

Zmiana monitora w trakcie działania aplikacji jest obsługiwana przez tryb
PerMonitorV2. Automatyczne testy nie sprawdzają położenia pikseli; każda zmiana
layoutu wymaga poniższej walidacji manualnej.

## Praktyczna weryfikacja

Układ został praktycznie zweryfikowany na tym samym komputerze w dwóch
konfiguracjach:

- 96 DPI / skalowanie Windows 100% / 1920 × 1080,
- 192 DPI / skalowanie Windows 200% / 3840 × 2160.

Zweryfikowane formularze:

- `MainForm`,
- `OrganizationsForm`,
- `OrganizationEditForm`,
- `ProfilesForm`,
- `ProfileEditForm`,
- `HistoryForm`,
- `AboutForm`.

Sprawdzono również dialogi wyświetlające `LICENSE` i
`THIRD-PARTY-NOTICES.md`. Rozmiary formularzy i przycisków zachowują ten sam
rozmiar logiczny przy 96 i 192 DPI. Dolne paski `OrganizationsForm`,
`ProfilesForm` oraz `HistoryForm` mają odpowiednio około 40 pikseli przy
96 DPI i 80 pikseli przy 192 DPI. Nie stwierdzono nakładania ani ucinania
kontrolek.

## Checklista testu manualnego

Dla każdej skali 100%, 125%, 150% i 200%:

1. Uruchomić aplikację ponownie po ustawieniu skali lub przenieść ją między
   monitorami o różnym DPI.
2. Sprawdzić `MainForm`: wszystkie sekcje, zakres Asset ID oraz przyciski
   `Historia...`, `Podgląd` i `Drukuj` są widoczne.
3. Otworzyć `HistoryForm`, zmienić rozmiar okna i sprawdzić pasek filtrów,
   tabelę, szczegóły oraz dolny pasek eksportu.
4. Otworzyć `ProfilesForm` i `OrganizationsForm`, zmienić ich rozmiar i
   sprawdzić listy oraz pełny układ przycisków.
5. Otworzyć oba formularze edycji i sprawdzić, czy etykiety nie nachodzą na
   pola, a `Zapisz` i `Anuluj` pozostają widoczne.
6. Otworzyć `AboutForm` i potwierdzić, że panel licencji bibliotek nie nachodzi
   na dolne przyciski.
7. Z `AboutForm` otworzyć `LICENSE` i `THIRD-PARTY-NOTICES.md`; sprawdzić
   zachowanie pustych wierszy, polskich znaków, zawijania i przewijania.
8. Sprawdzić kolejność przechodzenia klawiszem Tab oraz działanie Enter/Escape
   w formularzach dialogowych.

Zmiana geometrii `LabelPrintJob` nie jest elementem walidacji layoutu. Ten
renderer wymaga osobnych, ponownych testów sprzętowych po każdej zmianie jego
matematyki.

## Raport diagnostyczny

Aplikację można uruchomić z parametrem:

```text
EtykietyIT.exe --ui-diagnostics
```

Parametr nie jest dostępny w zwykłym interfejsie. Powoduje utworzenie raportu
`diagnostics/ui-diagnostics-<timestamp>.txt` w katalogu danych aktualnego trybu
Standard albo Portable. Katalog `diagnostics` jest tworzony tylko wtedy, gdy
diagnostyka została jawnie włączona.

Raport zawiera konfigurację High DPI aplikacji i systemu oraz parametry każdego
formularza po jego pokazaniu. Dla kontrolek zapisywane są rozmiary rzeczywiste,
`PreferredSize` i wartości przeliczone do ekwiwalentu 96 DPI. `MainForm`,
`ProfilesForm`, `AboutForm` oraz dialogi dokumentów otwierane z `AboutForm` są
raportowane szczegółowo. Pozostałe formularze zapisują dane formularza i jego
kontenerów layoutu.

Przy porównywaniu komputerów należy najpierw sprawdzić `DeviceDpi` i
`DpiScaleFrom96`, a następnie porównywać pola `Logical*At96Dpi`. Dwukrotnie
większe wymiary fizyczne przy 192 DPI nie są samodzielnie oznaką błędu.
