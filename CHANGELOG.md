# Changelog

## 3.0.0-rc.1

- przeniesienie aplikacji i zweryfikowanego silnika drukowania z PowerShell do
  C# / .NET 10 / Windows Forms,
- wbudowane i własne profile etykiet,
- profile organizacji z niezależną numeracją Asset ID,
- globalna kalibracja przypisana osobno do każdej drukarki,
- trwała historia zadań przekazanych do systemu drukowania Windows,
- wyszukiwanie i filtrowanie historii,
- eksport historii do CSV,
- eksport historii do XLSX,
- opcjonalny QR z dokładnie sformatowanym Asset ID,
- domyślne ustawienie QR osobne dla każdego profilu organizacji,
- dynamiczne dopasowanie QR do komórki, z minimum 4 dots/module,
- preflight drukowalności zależny od sterownika, geometrii i aktualnej treści,
  blokujący przewidywalne kolizje przed podglądem i drukiem,
- fizyczna walidacja QR i preflightu na DYMO LabelWriter 450 i 550,
- tryby przechowywania danych Standard i Portable.
