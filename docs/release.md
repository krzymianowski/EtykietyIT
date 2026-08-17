# Proces wydania

Ten dokument opisuje przygotowanie i weryfikację paczek Etykiety IT dla
Windows x64. Release candidate i wydanie finalne muszą powstawać z tego samego
powtarzalnego procesu.

## Wymagania

- 64-bitowy Windows,
- PowerShell 7 lub Windows PowerShell 5.1,
- .NET SDK wskazany w `global.json`,
- dostęp do źródeł pakietów NuGet podczas restore,
- zapisywalny katalog roboczy repozytorium.

Nie są wymagane zewnętrzne narzędzia do tworzenia ZIP-ów ani sum SHA-256.

## Przypięte SDK

Plik `global.json` wskazuje SDK 10.0.400 i ustawia `rollForward` na `disable`.
Oznacza to, że lokalny build i GitHub Actions muszą używać dokładnie tej wersji
SDK. Proces nie przejdzie automatycznie na nowszy patch ani feature band, dzięki
czemu wersja runtime'u w paczce self-contained nie zmieni się bez jawnej zmiany
`global.json` i ponownej walidacji release.

## Lokalny build release candidate

W katalogu głównym repozytorium uruchom:

```powershell
.\scripts\build-release.ps1
```

Skrypt wykonuje kolejno:

1. bezpieczne wyczyszczenie `artifacts/release`,
2. `dotnet restore`,
3. `dotnet build` w konfiguracji Release,
4. `dotnet test` w konfiguracji Release,
5. jeden publish aplikacji z profilem `win-x64-self-contained`,
6. przygotowanie dwóch katalogów staging z tych samych plików publish,
7. dołączenie dokumentacji i oficjalnych notices dystrybucji .NET,
8. utworzenie ZIP-ów Standard i Portable,
9. utworzenie `SHA256SUMS.txt`.

Opcjonalny parametr `ExpectedVersion` blokuje pakowanie, gdy oczekiwana wersja
nie odpowiada właściwości `Version` w projekcie:

```powershell
.\scripts\build-release.ps1 -ExpectedVersion 3.0.0-rc.1
```

Workflow wykorzystuje ten mechanizm do porównania wersji z nazwą tagu RC.

## Konfiguracja publish

Profil
`EtykietyIT/Properties/PublishProfiles/win-x64-self-contained.pubxml`
ustawia:

- `Configuration=Release`,
- `RuntimeIdentifier=win-x64`,
- `SelfContained=true`,
- `PublishSingleFile=false`,
- `PublishTrimmed=false`,
- `PublishReadyToRun=false`.

Single-file, trimming i ReadyToRun nie mogą zostać włączone dla v3.0 bez
osobnej analizy i pełnej ponownej walidacji artefaktów.

## Artefakty

Dla wersji `3.0.0-rc.1` powstają:

```text
artifacts/release/
  EtykietyIT-3.0.0-rc.1-win-x64.zip
  EtykietyIT-3.0.0-rc.1-win-x64-portable.zip
  SHA256SUMS.txt
  publish/
  staging/
```

Oba ZIP-y zawierają aplikację self-contained, wbudowane profile etykiet,
`README.md`, `LICENSE`, `THIRD-PARTY-NOTICES.md` oraz oficjalne pliki
`DOTNET-LICENSE.txt` i `DOTNET-THIRD-PARTY-NOTICES.txt` skopiowane z SDK
użytego do publish.

Wariant Portable różni się wyłącznie obecnością pustego pliku
`portable.mode`. Nie zawiera pustego katalogu `Data`; aplikacja tworzy
`Data/v3` podczas pierwszego uruchomienia.

## Weryfikacja SHA-256

Sumy obu ZIP-ów znajdują się w `SHA256SUMS.txt`. Lokalna kontrola pliku może
zostać wykonana poleceniem:

```powershell
Get-FileHash .\artifacts\release\EtykietyIT-*.zip -Algorithm SHA256
```

Wartości muszą odpowiadać sumom zapisanym w pliku i opublikowanym razem z
artefaktami.

## Wynik ręcznej walidacji 3.0.0-rc.1

Release candidate `3.0.0-rc.1` został ręcznie przetestowany na trzech
komputerach, w tym na komputerze z czystą instalacją Windows.

Potwierdzony wynik:

- tryb Standard — PASS,
- tryb Portable — PASS,
- uruchomienie self-contained — PASS,
- podstawowa funkcjonalność aplikacji — PASS.

## Checklista czystego komputera

### Paczka i uruchomienie

- [ ] ZIP został pobrany i jego SHA-256 jest poprawny.
- [ ] Aplikacja uruchamia się bez zainstalowanego .NET SDK.
- [ ] Aplikacja uruchamia się bez zainstalowanego .NET Desktop Runtime.
- [ ] About pokazuje oczekiwaną wersję użytkową.
- [ ] `Resources/Profiles`, LICENSE i notices są obecne.

### Tryb Standard

- [ ] Paczka Standard nie zawiera `portable.mode`.
- [ ] Dane powstają w `%LOCALAPPDATA%\EtykietyIT\v3`.
- [ ] Restart zachowuje ustawienia, organizacje i numerację.
- [ ] Dane użytkownika nie są zapisywane obok EXE.

### Tryb Portable

- [ ] Paczka Portable zawiera `portable.mode`.
- [ ] Dane powstają w `Data\v3` obok aplikacji.
- [ ] Dane użytkownika nie są zapisywane w `%LOCALAPPDATA%`.
- [ ] Restart zachowuje ustawienia, organizacje i numerację.
- [ ] Niezapisywalny katalog powoduje czytelny błąd bez przełączenia na
  Standard.

### Funkcje

- [ ] Tworzenie, edycja, duplikowanie i usuwanie organizacji.
- [ ] Niezależna numeracja co najmniej dwóch organizacji.
- [ ] Wbudowane i własne profile etykiet.
- [ ] Odczyt i zapis kalibracji wybranej drukarki.
- [ ] Preview z QR OFF i QR ON.
- [ ] Fizyczny druk z QR OFF i QR ON.
- [ ] Układ 2×2 z QR jest czytelny, a wszystkie QR skanują się poprawnie.
- [ ] Preflight 3×3 blokuje Preview i Print normalnym komunikatem.
- [ ] Odrzucony preflight nie zmienia historii ani NextAssetNumber.
- [ ] Udany Print zapisuje historię i aktualizuje NextAssetNumber.
- [ ] Wyszukiwanie oraz filtrowanie historii.
- [ ] Eksport CSV z polskimi znakami.
- [ ] Eksport XLSX bez zainstalowanego Microsoft Excel.
- [ ] Restart aplikacji zachowuje wszystkie dane.

### UI i High DPI

- [ ] Skalowanie Windows 100% / 96 DPI.
- [ ] Skalowanie Windows 125% / 120 DPI.
- [ ] Skalowanie Windows 150% / 144 DPI.
- [ ] Skalowanie Windows 200% / 192 DPI.
- [ ] Formularze pozostają czytelne po zmianie DPI monitora.
- [ ] Uruchomienie z `--ui-diagnostics` zapisuje raport.

## Finalizacja 3.0.0

Po zaakceptowaniu release candidate:

1. zmień wyłącznie `Version` z `3.0.0-rc.1` na `3.0.0`,
2. zmień nagłówek CHANGELOG na `3.0.0` i zaktualizuj status README,
3. wykonaj `dotnet build` i `dotnet test`,
4. uruchom `build-release.ps1 -ExpectedVersion 3.0.0`,
5. zweryfikuj finalne ZIP-y i sumy SHA-256,
6. utwórz tag `v3.0.0` na dokładnym, zaakceptowanym commicie,
7. dołącz oba ZIP-y i `SHA256SUMS.txt` do GitHub Release.

Tag i właściwość `Version` muszą być identyczne po usunięciu początkowego
znaku `v` z nazwy tagu.
