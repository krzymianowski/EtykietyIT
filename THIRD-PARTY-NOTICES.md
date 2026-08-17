# Third-party notices

Ten plik opisuje zależności NuGet używane przez repozytorium oraz składniki
.NET redystrybuowane w paczce self-contained. Pełne warunki licencji są
dostępne w podanych repozytoriach, paczkach i oficjalnych plikach dystrybucji.

## Runtime dependencies

### DocumentFormat.OpenXml

- Version: 3.5.1
- License: MIT
- Project: Open XML SDK
- Repository: https://github.com/dotnet/Open-XML-SDK
- NuGet: https://www.nuget.org/packages/DocumentFormat.OpenXml/3.5.1

Biblioteka jest używana do tworzenia i sprawdzania eksportów historii w
formacie XLSX bez wymagania instalacji Microsoft Excel.
Pakiet korzysta również z `DocumentFormat.OpenXml.Framework 3.5.1`, będącego
częścią tego samego projektu Open XML SDK i objętego tą samą licencją MIT.

### Net.Codecrete.QrCodeGenerator

- Version: 3.1.0
- License: MIT
- Project: QR Code Generator for .NET
- Repository: https://github.com/manuelbl/QrCodeGenerator
- NuGet: https://www.nuget.org/packages/Net.Codecrete.QrCodeGenerator/3.1.0

Biblioteka jest używana do kodowania Asset ID w macierz QR. Aplikacja rysuje
moduły bezpośrednio przez `System.Drawing.Graphics`; nie używa bitmapowego
renderera biblioteki.

### .NET 10 Runtime and Windows Desktop Runtime

- SDK used for release: 10.0.400
- Runtime included by the current self-contained publish: 10.0.11
- Distribution: self-contained Windows x64
- License for the Windows binary distribution: Microsoft .NET Library License
- License information: https://github.com/dotnet/core/blob/main/license-information.md
- License terms: https://dotnet.microsoft.com/dotnet_library_license.htm

Paczki self-contained redystrybuują elementy .NET Runtime oraz Windows Desktop
Runtime potrzebne do uruchomienia aplikacji WinForms bez osobnej instalacji
.NET. Proces wydania kopiuje bez zmian oficjalne pliki z dystrybucji
przypiętego SDK użytej do publikacji:

- `DOTNET-LICENSE.txt` — oficjalne warunki licencyjne dystrybucji .NET dla
  Windows,
- `DOTNET-THIRD-PARTY-NOTICES.txt` — oficjalne informacje o komponentach
  zewnętrznych zawartych w dystrybucji .NET.

Pliki te są dołączane osobno do każdego ZIP-a release i są miarodajne dla
redystrybuowanych binariów .NET. Nie zastępują licencji MIT samej aplikacji.

## Development dependencies

Poniższe pakiety są używane wyłącznie podczas budowania i uruchamiania testów.
Nie są wymagane przez opublikowaną aplikację.

### MSTest

- Version: 4.0.2
- License: MIT
- Project: MSTest / Microsoft Test Platform
- Repository: https://github.com/microsoft/testfx
- NuGet: https://www.nuget.org/packages/MSTest/4.0.2
