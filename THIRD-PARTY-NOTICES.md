# Third-party notices

Ten plik opisuje bezpośrednie zależności NuGet używane przez repozytorium.
Pełne warunki licencji są dostępne w podanych repozytoriach projektów oraz
w paczkach NuGet.

## Runtime dependencies

### DocumentFormat.OpenXml

- Version: 3.5.1
- License: MIT
- Project: Open XML SDK
- Repository: https://github.com/dotnet/Open-XML-SDK
- NuGet: https://www.nuget.org/packages/DocumentFormat.OpenXml/3.5.1

Biblioteka jest używana do tworzenia i sprawdzania eksportów historii w
formacie XLSX bez wymagania instalacji Microsoft Excel.

### Net.Codecrete.QrCodeGenerator

- Version: 3.1.0
- License: MIT
- Project: QR Code Generator for .NET
- Repository: https://github.com/manuelbl/QrCodeGenerator
- NuGet: https://www.nuget.org/packages/Net.Codecrete.QrCodeGenerator/3.1.0

Biblioteka jest używana do kodowania Asset ID w macierz QR. Aplikacja rysuje
moduły bezpośrednio przez `System.Drawing.Graphics`; nie używa bitmapowego
renderera biblioteki.

## Development dependencies

Poniższe pakiety są używane wyłącznie podczas budowania i uruchamiania testów.
Nie są wymagane przez opublikowaną aplikację.

### MSTest

- Version: 4.0.2
- License: MIT
- Project: MSTest / Microsoft Test Platform
- Repository: https://github.com/microsoft/testfx
- NuGet: https://www.nuget.org/packages/MSTest/4.0.2
