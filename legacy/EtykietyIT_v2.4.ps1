#requires -Version 5.1
<#
  Etykiety IT v2.4
  Aplikacja WinForms do drukowania etykiet inwentarzowych.

  Poprawki v2.4:
  - poprawione mapowanie fizycznej szerokości etykiety na obszar drukowalny,
  - w orientacji Landscape używany jest bezpośrednio HardMarginX/HardMarginY,
  - usunięto błędne nadpisywanie marginesów wartościami PrintableArea.X/Y,
  - linia podziału jest liczona z rzeczywistego PageSettings.Bounds,
  - podgląd pozostaje w naturalnym układzie fizycznej strony,
  - zachowany pełny tryb diagnostyczny z v2.3.

  Dane aplikacji:
    %LOCALAPPDATA%\EtykietyIT\settings.json
    %LOCALAPPDATA%\EtykietyIT\history_v2.csv
#>

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
[System.Windows.Forms.Application]::EnableVisualStyles()

# ---------------------------------------------------------
# Stałe
# ---------------------------------------------------------
$Prefix = 'IT-'
$Digits = 6
$Company = 'Dolnośląskie Młyny S.A.'
$DefaultNextNumber = 11

$DataDir = if ($env:LOCALAPPDATA) {
    Join-Path $env:LOCALAPPDATA 'EtykietyIT'
} else {
    Join-Path $PSScriptRoot 'Dane'
}
$SettingsPath = Join-Path $DataDir 'settings.json'
$OldHistoryPath = Join-Path $DataDir 'history.csv'
$HistoryPath = Join-Path $DataDir 'history_v2.csv'
$DiagnosticsDir = Join-Path $DataDir 'diagnostics'

if (-not (Test-Path $DataDir)) {
    New-Item -ItemType Directory -Path $DataDir -Force | Out-Null
}

if (-not (Test-Path $DiagnosticsDir)) {
    New-Item -ItemType Directory -Path $DiagnosticsDir -Force | Out-Null
}

# ---------------------------------------------------------
# Profile
# Uwaga: to profile aplikacji, nie deklaracja konkretnych SKU DYMO.
# Każdy rozmiar można zmienić ręcznie.
# ---------------------------------------------------------
$BuiltInProfiles = @(
    [pscustomobject]@{
        Name = '89 × 41 mm — 2 szt. w poziomie'
        WidthMm = 89.0
        HeightMm = 41.0
        Columns = 2
        Rows = 1
    },
    [pscustomobject]@{
        Name = '89 × 41 mm — 1 szt.'
        WidthMm = 89.0
        HeightMm = 41.0
        Columns = 1
        Rows = 1
    },
    [pscustomobject]@{
        Name = '54 × 25 mm — 1 szt.'
        WidthMm = 54.0
        HeightMm = 25.0
        Columns = 1
        Rows = 1
    },
    [pscustomobject]@{
        Name = '57 × 32 mm — 1 szt.'
        WidthMm = 57.0
        HeightMm = 32.0
        Columns = 1
        Rows = 1
    },
    [pscustomobject]@{
        Name = '70 × 54 mm — 1 szt.'
        WidthMm = 70.0
        HeightMm = 54.0
        Columns = 1
        Rows = 1
    },
    [pscustomobject]@{
        Name = 'Własny rozmiar'
        WidthMm = 89.0
        HeightMm = 41.0
        Columns = 2
        Rows = 1
    }
)

function Format-AssetId {
    param([int]$Number)
    return $Prefix + $Number.ToString("D$Digits")
}

function Get-InstalledPrinters {
    @(
        [System.Drawing.Printing.PrinterSettings]::InstalledPrinters |
        ForEach-Object { [string]$_ }
    )
}

function Get-DefaultSettings {
    [pscustomobject]@{
        NextNumber    = $DefaultNextNumber
        PrinterName   = ''
        ProfileName   = '89 × 41 mm — 2 szt. w poziomie'
        CustomWidthMm = 89.0
        CustomHeightMm = 41.0
        CustomColumns = 2
        CustomRows    = 1
        DrawCutLines  = $true
    }
}

function Load-Settings {
    if (Test-Path $SettingsPath) {
        try {
            $raw = Get-Content -LiteralPath $SettingsPath -Raw -Encoding UTF8 | ConvertFrom-Json
            $defaults = Get-DefaultSettings

            foreach ($prop in $defaults.PSObject.Properties.Name) {
                if ($null -eq $raw.$prop) {
                    $raw | Add-Member -NotePropertyName $prop -NotePropertyValue $defaults.$prop -Force
                }
            }

            if ([int]$raw.NextNumber -lt 0) {
                $raw.NextNumber = $DefaultNextNumber
            }

            return $raw
        }
        catch {
            return Get-DefaultSettings
        }
    }
    return Get-DefaultSettings
}

function Save-Settings {
    param(
        [int]$NextNumber,
        [string]$PrinterName,
        [string]$ProfileName,
        [double]$CustomWidthMm,
        [double]$CustomHeightMm,
        [int]$CustomColumns,
        [int]$CustomRows,
        [bool]$DrawCutLines
    )

    [pscustomobject]@{
        NextNumber     = $NextNumber
        PrinterName    = $PrinterName
        ProfileName    = $ProfileName
        CustomWidthMm  = $CustomWidthMm
        CustomHeightMm = $CustomHeightMm
        CustomColumns  = $CustomColumns
        CustomRows     = $CustomRows
        DrawCutLines   = $DrawCutLines
    } | ConvertTo-Json | Set-Content -LiteralPath $SettingsPath -Encoding UTF8
}

function Ensure-HistoryFile {
    if (Test-Path $HistoryPath) { return }

    # Migracja historii v1, jeśli istnieje.
    if (Test-Path $OldHistoryPath) {
        try {
            $old = @(Import-Csv -LiteralPath $OldHistoryPath -Delimiter ';' -Encoding UTF8)
            if ($old.Count -gt 0) {
                $migrated = foreach ($r in $old) {
                    [pscustomobject]@{
                        Timestamp      = $r.Timestamp
                        StartNumber    = $r.StartNumber
                        EndNumber      = $r.EndNumber
                        FirstId        = $r.FirstId
                        LastId         = $r.LastId
                        Quantity       = $r.Quantity
                        PhysicalLabels = $r.PhysicalLabels
                        Printer        = $r.Printer
                        Profile        = '89 × 41 mm — 2 szt. w poziomie'
                        WidthMm        = '89'
                        HeightMm       = '41'
                        Columns        = '2'
                        Rows           = '1'
                    }
                }
                $migrated | Export-Csv -LiteralPath $HistoryPath -Delimiter ';' -Encoding UTF8 -NoTypeInformation
                return
            }
        }
        catch {}
    }

    'Timestamp;StartNumber;EndNumber;FirstId;LastId;Quantity;PhysicalLabels;Printer;Profile;WidthMm;HeightMm;Columns;Rows' |
        Set-Content -LiteralPath $HistoryPath -Encoding UTF8
}

function Get-History {
    Ensure-HistoryFile
    try {
        @(Import-Csv -LiteralPath $HistoryPath -Delimiter ';' -Encoding UTF8)
    }
    catch {
        @()
    }
}

function Add-History {
    param(
        [int]$StartNumber,
        [int]$EndNumber,
        [int]$Quantity,
        [int]$PhysicalLabels,
        [string]$Printer,
        [string]$Profile,
        [double]$WidthMm,
        [double]$HeightMm,
        [int]$Columns,
        [int]$Rows
    )

    Ensure-HistoryFile

    [pscustomobject]@{
        Timestamp      = (Get-Date).ToString('yyyy-MM-dd HH:mm:ss')
        StartNumber    = $StartNumber
        EndNumber      = $EndNumber
        FirstId        = Format-AssetId $StartNumber
        LastId         = Format-AssetId $EndNumber
        Quantity       = $Quantity
        PhysicalLabels = $PhysicalLabels
        Printer        = $Printer
        Profile        = $Profile
        WidthMm        = $WidthMm.ToString('0.##', [Globalization.CultureInfo]::InvariantCulture)
        HeightMm       = $HeightMm.ToString('0.##', [Globalization.CultureInfo]::InvariantCulture)
        Columns        = $Columns
        Rows           = $Rows
    } | Export-Csv -LiteralPath $HistoryPath -Delimiter ';' -Encoding UTF8 -NoTypeInformation -Append
}

function Find-Overlap {
    param(
        [int]$StartNumber,
        [int]$EndNumber
    )

    foreach ($row in (Get-History)) {
        $oldStart = 0
        $oldEnd = 0
        if (
            [int]::TryParse([string]$row.StartNumber, [ref]$oldStart) -and
            [int]::TryParse([string]$row.EndNumber, [ref]$oldEnd)
        ) {
            if ($StartNumber -le $oldEnd -and $EndNumber -ge $oldStart) {
                return $row
            }
        }
    }
    return $null
}

# ---------------------------------------------------------
# Pomocnicze skalowanie tekstu
# ---------------------------------------------------------
function New-FittingFont {
    param(
        [System.Drawing.Graphics]$Graphics,
        [string]$Text,
        [System.Drawing.RectangleF]$Rect,
        [string]$FontName,
        [System.Drawing.FontStyle]$Style,
        [single]$MaxPt,
        [single]$MinPt
    )

    $size = $MaxPt
    while ($size -ge $MinPt) {
        $font = [System.Drawing.Font]::new($FontName, $size, $Style)
        $measured = $Graphics.MeasureString($Text, $font, 10000)

        # MeasureString podaje rozmiar w jednostkach Graphics.PageUnit.
        if ($measured.Width -le $Rect.Width -and $measured.Height -le ($Rect.Height * 1.15)) {
            return $font
        }

        $font.Dispose()
        $size -= 0.5
    }

    return [System.Drawing.Font]::new($FontName, $MinPt, $Style)
}

# ---------------------------------------------------------
# Silnik druku
# ---------------------------------------------------------
function New-LabelPrintDocument {
    param(
        [string]$PrinterName,
        [int]$StartNumber,
        [int]$Quantity,
        [double]$WidthMm,
        [double]$HeightMm,
        [int]$Columns,
        [int]$Rows,
        [bool]$DrawCutLines
    )

    $installed = @(Get-InstalledPrinters)
    if ($installed -notcontains $PrinterName) {
        throw "Nie znaleziono drukarki '$PrinterName'."
    }

    if ($WidthMm -lt 20 -or $HeightMm -lt 15) {
        throw 'Rozmiar etykiety jest zbyt mały.'
    }
    if ($Columns -lt 1 -or $Rows -lt 1) {
        throw 'Układ etykiet musi mieć co najmniej 1 kolumnę i 1 wiersz.'
    }

    # PaperSize używa 1/100 cala.
    $targetWidth  = [int][Math]::Round(($WidthMm / 25.4) * 100)
    $targetHeight = [int][Math]::Round(($HeightMm / 25.4) * 100)
    $tolerance = 8

    $printDoc = [System.Drawing.Printing.PrintDocument]::new()
    $printDoc.DocumentName = 'Etykiety inwentarzowe IT'
    $printDoc.PrinterSettings.PrinterName = $PrinterName
    $printDoc.OriginAtMargins = $false

    $selectedPaper = $null
    $landscape = $false

    foreach ($paper in $printDoc.PrinterSettings.PaperSizes) {
        $normal =
            ([Math]::Abs($paper.Width  - $targetWidth)  -le $tolerance) -and
            ([Math]::Abs($paper.Height - $targetHeight) -le $tolerance)

        $rotated =
            ([Math]::Abs($paper.Width  - $targetHeight) -le $tolerance) -and
            ([Math]::Abs($paper.Height - $targetWidth)  -le $tolerance)

        if ($normal) {
            $selectedPaper = $paper
            $landscape = $false
            break
        }
        if ($rotated) {
            $selectedPaper = $paper
            $landscape = $true
            break
        }
    }

    if (-not $selectedPaper) {
        $selectedPaper = [System.Drawing.Printing.PaperSize]::new(
            ("{0} x {1} mm" -f $WidthMm, $HeightMm),
            $targetWidth,
            $targetHeight
        )
        $landscape = $false
    }

    $printDoc.DefaultPageSettings.PaperSize = $selectedPaper
    $printDoc.DefaultPageSettings.Landscape = $landscape
    $printDoc.DefaultPageSettings.Margins =
        [System.Drawing.Printing.Margins]::new(0, 0, 0, 0)

    $state = @{ Index = 0 }

    $resources = @{
        Center = [System.Drawing.StringFormat]::new()
        CutPen = [System.Drawing.Pen]::new([System.Drawing.Color]::Black, 0.25)
    }
    $resources.Center.Alignment = [System.Drawing.StringAlignment]::Center
    $resources.Center.LineAlignment = [System.Drawing.StringAlignment]::Center
    $resources.CutPen.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash

    $printDoc.add_BeginPrint({
        $state.Index = 0
    }.GetNewClosure())

    $printDoc.add_PrintPage({
        param($sender, $e)

        $g = $e.Graphics
        $g.PageUnit = [System.Drawing.GraphicsUnit]::Millimeter
        $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit

        # -------------------------------------------------------------
        # Układ współrzędnych:
        # PrintDocument rysuje od początku OBSZARU DRUKOWALNEGO, a nie
        # od fizycznej krawędzi etykiety. W DYMO margines sprzętowy może
        # być na tyle duży, że 44,5 mm od początku obszaru drukowalnego
        # nie jest fizycznym środkiem etykiety 89 mm.
        #
        # Dlatego:
        # 1. przesuwamy układ do fizycznego początku etykiety,
        # 2. linie podziału rysujemy względem pełnego WidthMm/HeightMm,
        # 3. zawartość ograniczamy do obszaru, który drukarka potrafi
        #    rzeczywiście zadrukować.
        # -------------------------------------------------------------

        # -------------------------------------------------------------
        # v2.4:
        # W diagnostyce DYMO 450/550 w Landscape sterownik zwraca:
        #
        #   Bounds          ~ 88,90 x 41,40 mm
        #   HardMarginX     ~ 5,84 mm
        #   HardMarginY     ~ 1,02 mm
        #   VisibleClip     ~ 81,53 x 38,86 mm
        #
        # PrintableArea.X/Y pozostają natomiast zapisane w osiach
        # pierwotnej (portretowej) orientacji i NIE mogą zastępować
        # HardMarginX/Y po obróceniu strony.
        # -------------------------------------------------------------

        [single]$pageW = $e.PageSettings.Bounds.Width * 0.254
        [single]$pageH = $e.PageSettings.Bounds.Height * 0.254

        if ($pageW -le 0) { $pageW = [single]$WidthMm }
        if ($pageH -le 0) { $pageH = [single]$HeightMm }

        # Ustal, czy PrintPage jest wykonywany dla podglądu.
        $isPreview = $false
        try {
            $controllerName = $sender.PrintController.GetType().Name
            $isPreview = ($controllerName -match 'Preview')
        }
        catch {}

        if ($isPreview) {
            # PrintPreviewDialog renderuje całą stronę, więc nie stosujemy
            # kompensacji sprzętowego marginesu. Podgląd ma pokazywać
            # fizyczny, równy podział strony.
            [single]$hardLeft = 0.0
            [single]$hardTop = 0.0
            [single]$hardRight = 0.0
            [single]$hardBottom = 0.0

            [single]$safeEdgeX = [Math]::Max(1.0, [Math]::Min(1.5, $pageW * 0.02))
            [single]$safeEdgeY = [Math]::Max(1.0, [Math]::Min(1.5, $pageH * 0.035))
        }
        else {
            # Na realnym wydruku Graphics(0,0) odpowiada pierwszemu
            # drukowalnemu punktowi, a nie fizycznej krawędzi etykiety.
            [single]$hardLeft = $e.PageSettings.HardMarginX * 0.254
            [single]$hardTop  = $e.PageSettings.HardMarginY * 0.254

            # VisibleClipBounds po ustawieniu PageUnit=Millimeter daje
            # rzeczywisty rozmiar obszaru, który można zadrukować.
            [single]$printableW = $g.VisibleClipBounds.Width
            [single]$printableH = $g.VisibleClipBounds.Height

            [single]$hardRight = $pageW - $hardLeft - $printableW
            [single]$hardBottom = $pageH - $hardTop - $printableH

            # Zabezpieczenie dla sterowników zwracających nietypowe dane.
            if ($hardLeft -lt 0 -or $hardLeft -gt ($pageW / 2.0)) {
                $hardLeft = 0.0
            }
            if ($hardTop -lt 0 -or $hardTop -gt ($pageH / 2.0)) {
                $hardTop = 0.0
            }
            if ($hardRight -lt 0 -or $hardRight -gt ($pageW / 2.0)) {
                $hardRight = 0.0
            }
            if ($hardBottom -lt 0 -or $hardBottom -gt ($pageH / 2.0)) {
                $hardBottom = 0.0
            }

            # Dla zawartości stosujemy symetryczny bezpieczny margines.
            # Sama linia cięcia pozostaje dokładnie w fizycznym środku.
            [single]$safeEdgeX = [Math]::Max(
                1.0,
                [Math]::Max($hardLeft, $hardRight)
            )
            [single]$safeEdgeY = [Math]::Max(
                1.0,
                [Math]::Max($hardTop, $hardBottom)
            )

            # Najważniejsza korekta:
            # użytkowe X=0 zostaje przesunięte do fizycznej lewej
            # krawędzi etykiety.
            $g.TranslateTransform(-$hardLeft, -$hardTop)
        }

        # Komórki liczymy z RZECZYWISTEGO rozmiaru strony zwróconego
        # przez PageSettings.Bounds. Dla badanego formatu daje to
        # 88,90 / 2 = 44,45 mm.
        [single]$cellW = $pageW / $Columns
        [single]$cellH = $pageH / $Rows

        # Linie cięcia są w prawdziwym środku fizycznej etykiety.
        if ($DrawCutLines) {
            if ($Columns -gt 1) {
                for ($c = 1; $c -lt $Columns; $c++) {
                    [single]$cutX = $cellW * $c
                    $g.DrawLine(
                        $resources.CutPen,
                        $cutX,
                        $safeEdgeY,
                        $cutX,
                        [single]($pageH - $safeEdgeY)
                    )
                }
            }
            if ($Rows -gt 1) {
                for ($r = 1; $r -lt $Rows; $r++) {
                    [single]$cutY = $cellH * $r
                    $g.DrawLine(
                        $resources.CutPen,
                        $safeEdgeX,
                        $cutY,
                        [single]($pageW - $safeEdgeX),
                        $cutY
                    )
                }
            }
        }

        $slotsPerPage = $Columns * $Rows

        for ($slot = 0; $slot -lt $slotsPerPage; $slot++) {
            if ($state.Index -ge $Quantity) { break }

            $col = $slot % $Columns
            $row = [Math]::Floor($slot / $Columns)

            [single]$cellX0 = $col * $cellW
            [single]$cellX1 = ($col + 1) * $cellW
            [single]$cellY0 = $row * $cellH
            [single]$cellY1 = ($row + 1) * $cellH

            # Niewielki odstęp od linii cięcia / granicy komórki.
            [single]$cellPadX = [Math]::Max(0.8, [Math]::Min(1.5, $cellW * 0.035))
            [single]$cellPadY = [Math]::Max(0.8, [Math]::Min(1.2, $cellH * 0.035))

            # Przecięcie komórki z bezpiecznym obszarem drukowalnym.
            [single]$layoutX0 = [Math]::Max(($cellX0 + $cellPadX), $safeEdgeX)
            [single]$layoutX1 = [Math]::Min(($cellX1 - $cellPadX), ($pageW - $safeEdgeX))
            [single]$layoutY0 = [Math]::Max(($cellY0 + $cellPadY), $safeEdgeY)
            [single]$layoutY1 = [Math]::Min(($cellY1 - $cellPadY), ($pageH - $safeEdgeY))

            [single]$layoutW = $layoutX1 - $layoutX0
            [single]$layoutH = $layoutY1 - $layoutY0

            if ($layoutW -lt 10.0 -or $layoutH -lt 10.0) {
                throw "Obszar drukowalny sterownika jest zbyt mały dla wybranego układu etykiet."
            }

            $number = $StartNumber + $state.Index
            $assetId = Format-AssetId $number

            # Proporcje pionowe liczone już wewnątrz realnego obszaru
            # dostępnego dla danej małej etykiety.
            [single]$titleY = $layoutY0 + ($layoutH * 0.07)
            [single]$titleH = [Math]::Max(3.0, $layoutH * 0.14)

            [single]$numberY = $layoutY0 + ($layoutH * 0.23)
            [single]$numberH = [Math]::Max(6.0, $layoutH * 0.38)

            [single]$barH = [Math]::Max(5.0, $layoutH * 0.19)
            [single]$barY = $layoutY1 - $barH - [Math]::Max(0.8, $layoutH * 0.04)

            $titleRect = [System.Drawing.RectangleF]::new(
                $layoutX0, $titleY, $layoutW, $titleH
            )
            $numberRect = [System.Drawing.RectangleF]::new(
                $layoutX0, $numberY, $layoutW, $numberH
            )
            $barRect = [System.Drawing.RectangleF]::new(
                $layoutX0, $barY, $layoutW, $barH
            )

            # Maksymalne czcionki zależne od rzeczywistego pola.
            [single]$titleMax = [Math]::Min(9.0, [Math]::Max(5.0, $layoutH * 0.18))
            [single]$numberMax = [Math]::Min(24.0, [Math]::Max(8.0, $layoutH * 0.50))
            [single]$companyMax = [Math]::Min(9.0, [Math]::Max(4.5, $layoutH * 0.16))

            $titleFont = New-FittingFont `
                -Graphics $g `
                -Text 'Nr inwentarzowy' `
                -Rect $titleRect `
                -FontName 'Arial' `
                -Style ([System.Drawing.FontStyle]::Regular) `
                -MaxPt $titleMax `
                -MinPt 4.0

            $numberFont = New-FittingFont `
                -Graphics $g `
                -Text $assetId `
                -Rect $numberRect `
                -FontName 'Arial' `
                -Style ([System.Drawing.FontStyle]::Bold) `
                -MaxPt $numberMax `
                -MinPt 6.0

            $companyFont = New-FittingFont `
                -Graphics $g `
                -Text $Company `
                -Rect $barRect `
                -FontName 'Arial' `
                -Style ([System.Drawing.FontStyle]::Bold) `
                -MaxPt $companyMax `
                -MinPt 3.5

            try {
                $g.DrawString(
                    'Nr inwentarzowy',
                    $titleFont,
                    [System.Drawing.Brushes]::Black,
                    $titleRect,
                    $resources.Center
                )

                $g.DrawString(
                    $assetId,
                    $numberFont,
                    [System.Drawing.Brushes]::Black,
                    $numberRect,
                    $resources.Center
                )

                $g.FillRectangle([System.Drawing.Brushes]::Black, $barRect)
                $g.DrawString(
                    $Company,
                    $companyFont,
                    [System.Drawing.Brushes]::White,
                    $barRect,
                    $resources.Center
                )
            }
            finally {
                $titleFont.Dispose()
                $numberFont.Dispose()
                $companyFont.Dispose()
            }

            $state.Index++
        }

        $e.HasMorePages = ($state.Index -lt $Quantity)
    }.GetNewClosure())

    [pscustomobject]@{
        Document    = $printDoc
        Resources   = $resources
        PaperName   = $selectedPaper.PaperName
        IsLandscape = $landscape
    }
}

function Dispose-PrintPackage {
    param($Package)
    if ($null -eq $Package) { return }

    try { $Package.Resources.Center.Dispose() } catch {}
    try { $Package.Resources.CutPen.Dispose() } catch {}
    try { $Package.Document.Dispose() } catch {}
}


# ---------------------------------------------------------
# Diagnostyka sterownika / strony
# ---------------------------------------------------------
function Convert-HundredthsInchToMm {
    param([double]$Value)
    return ($Value * 0.254)
}

function Format-RectDiagnostic {
    param($Rect, [string]$Unit = '1/100 cala')
    if ($null -eq $Rect) { return '<brak>' }

    if ($Unit -eq '1/100 cala') {
        $xMm = Convert-HundredthsInchToMm $Rect.X
        $yMm = Convert-HundredthsInchToMm $Rect.Y
        $wMm = Convert-HundredthsInchToMm $Rect.Width
        $hMm = Convert-HundredthsInchToMm $Rect.Height

        return ("X={0:0.##}, Y={1:0.##}, W={2:0.##}, H={3:0.##} [1/100 in]  =>  X={4:0.00}, Y={5:0.00}, W={6:0.00}, H={7:0.00} mm" -f `
            $Rect.X, $Rect.Y, $Rect.Width, $Rect.Height, $xMm, $yMm, $wMm, $hMm)
    }

    return ("X={0:0.##}, Y={1:0.##}, W={2:0.##}, H={3:0.##} [{4}]" -f `
        $Rect.X, $Rect.Y, $Rect.Width, $Rect.Height, $Unit)
}

function Get-DiagnosticPrintDocument {
    param(
        [string]$PrinterName,
        [double]$WidthMm,
        [double]$HeightMm
    )

    $installed = @(Get-InstalledPrinters)
    if ($installed -notcontains $PrinterName) {
        throw "Nie znaleziono drukarki '$PrinterName'."
    }

    $targetWidth  = [int][Math]::Round(($WidthMm / 25.4) * 100)
    $targetHeight = [int][Math]::Round(($HeightMm / 25.4) * 100)
    $tolerance = 8

    $doc = [System.Drawing.Printing.PrintDocument]::new()
    $doc.DocumentName = 'Etykiety IT - diagnostyka'
    $doc.PrinterSettings.PrinterName = $PrinterName
    $doc.OriginAtMargins = $false

    $selectedPaper = $null
    $landscape = $false

    foreach ($paper in $doc.PrinterSettings.PaperSizes) {
        $normal =
            ([Math]::Abs($paper.Width  - $targetWidth)  -le $tolerance) -and
            ([Math]::Abs($paper.Height - $targetHeight) -le $tolerance)

        $rotated =
            ([Math]::Abs($paper.Width  - $targetHeight) -le $tolerance) -and
            ([Math]::Abs($paper.Height - $targetWidth)  -le $tolerance)

        if ($normal) {
            $selectedPaper = $paper
            $landscape = $false
            break
        }

        if ($rotated) {
            $selectedPaper = $paper
            $landscape = $true
            break
        }
    }

    if (-not $selectedPaper) {
        $selectedPaper = [System.Drawing.Printing.PaperSize]::new(
            ("{0} x {1} mm" -f $WidthMm, $HeightMm),
            $targetWidth,
            $targetHeight
        )
        $landscape = $false
    }

    $doc.DefaultPageSettings.PaperSize = $selectedPaper
    $doc.DefaultPageSettings.Landscape = $landscape
    $doc.DefaultPageSettings.Margins =
        [System.Drawing.Printing.Margins]::new(0, 0, 0, 0)

    return [pscustomobject]@{
        Document      = $doc
        SelectedPaper = $selectedPaper
        Landscape     = $landscape
    }
}

function Get-StaticDiagnosticReport {
    param(
        [string]$PrinterName,
        [double]$WidthMm,
        [double]$HeightMm,
        [string]$ProfileName,
        [int]$Columns,
        [int]$Rows
    )

    $pkg = $null
    try {
        $pkg = Get-DiagnosticPrintDocument `
            -PrinterName $PrinterName `
            -WidthMm $WidthMm `
            -HeightMm $HeightMm

        $doc = $pkg.Document
        $ps = $doc.DefaultPageSettings
        $printer = $doc.PrinterSettings
        $paper = $ps.PaperSize
        $pa = $ps.PrintableArea
        $bounds = $ps.Bounds

        $paperWmm = Convert-HundredthsInchToMm $paper.Width
        $paperHmm = Convert-HundredthsInchToMm $paper.Height
        $hardXmm = Convert-HundredthsInchToMm $ps.HardMarginX
        $hardYmm = Convert-HundredthsInchToMm $ps.HardMarginY

        $centerRequested = $WidthMm / 2.0
        $centerPaper = $paperWmm / 2.0

        $lines = @(
            'ETYKIETY IT v2.3 - RAPORT DIAGNOSTYCZNY',
            ('Data: {0}' -f (Get-Date).ToString('yyyy-MM-dd HH:mm:ss')),
            '',
            '=== ŻĄDANY FORMAT APLIKACJI ===',
            ('Profil: {0}' -f $ProfileName),
            ('Rozmiar: {0:0.00} x {1:0.00} mm' -f $WidthMm, $HeightMm),
            ('Układ: {0} kol. x {1} wiersz(e)' -f $Columns, $Rows),
            ('Oczekiwany fizyczny środek X: {0:0.00} mm' -f $centerRequested),
            '',
            '=== DRUKARKA ===',
            ('PrinterName: {0}' -f $PrinterName),
            ('IsValid: {0}' -f $printer.IsValid),
            ('IsDefaultPrinter: {0}' -f $printer.IsDefaultPrinter),
            ('SupportsColor: {0}' -f $printer.SupportsColor),
            ('MaximumCopies: {0}' -f $printer.MaximumCopies),
            '',
            '=== WYBRANY FORMAT STEROWNIKA ===',
            ('PaperName: {0}' -f $paper.PaperName),
            ('Paper Kind / RawKind: {0} / {1}' -f $paper.Kind, $paper.RawKind),
            ('PaperSize raw: W={0}, H={1} [1/100 in]' -f $paper.Width, $paper.Height),
            ('PaperSize mm: W={0:0.00}, H={1:0.00} mm' -f $paperWmm, $paperHmm),
            ('Środek PaperSize X: {0:0.00} mm' -f $centerPaper),
            ('Landscape: {0}' -f $ps.Landscape),
            ('OriginAtMargins: {0}' -f $doc.OriginAtMargins),
            '',
            '=== PAGE SETTINGS ===',
            ('Bounds: {0}' -f (Format-RectDiagnostic $bounds)),
            ('PrintableArea: {0}' -f (Format-RectDiagnostic $pa)),
            ('HardMarginX: {0:0.##} [1/100 in] = {1:0.00} mm' -f $ps.HardMarginX, $hardXmm),
            ('HardMarginY: {0:0.##} [1/100 in] = {1:0.00} mm' -f $ps.HardMarginY, $hardYmm),
            ('Margins: L={0}, R={1}, T={2}, B={3} [1/100 in]' -f `
                $ps.Margins.Left, $ps.Margins.Right, $ps.Margins.Top, $ps.Margins.Bottom),
            '',
            '=== ROZDZIELCZOŚĆ ===',
            ('PrinterResolution Kind: {0}' -f $ps.PrinterResolution.Kind),
            ('PrinterResolution X/Y: {0} / {1} dpi' -f `
                $ps.PrinterResolution.X, $ps.PrinterResolution.Y),
            '',
            '=== LISTA FORMATÓW STEROWNIKA ZBLIŻONYCH DO ŻĄDANEGO ==='
        )

        $near = @()
        foreach ($p in $printer.PaperSizes) {
            $wmm = Convert-HundredthsInchToMm $p.Width
            $hmm = Convert-HundredthsInchToMm $p.Height

            $deltaNormal = [Math]::Abs($wmm - $WidthMm) + [Math]::Abs($hmm - $HeightMm)
            $deltaRot = [Math]::Abs($wmm - $HeightMm) + [Math]::Abs($hmm - $WidthMm)
            $delta = [Math]::Min($deltaNormal, $deltaRot)

            if ($delta -le 20.0) {
                $near += [pscustomobject]@{
                    Delta = $delta
                    Text = ('{0}: {1:0.00} x {2:0.00} mm | raw {3} x {4} | RawKind={5}' -f `
                        $p.PaperName, $wmm, $hmm, $p.Width, $p.Height, $p.RawKind)
                }
            }
        }

        foreach ($n in ($near | Sort-Object Delta | Select-Object -First 12)) {
            $lines += ('- ' + $n.Text)
        }

        return ($lines -join "`r`n")
    }
    finally {
        if ($pkg -and $pkg.Document) {
            try { $pkg.Document.Dispose() } catch {}
        }
    }
}

function Show-DiagnosticWindow {
    param(
        [string]$PrinterName,
        [double]$WidthMm,
        [double]$HeightMm,
        [string]$ProfileName,
        [int]$Columns,
        [int]$Rows,
        [System.Windows.Forms.Form]$Owner
    )

    $diagForm = [System.Windows.Forms.Form]::new()
    $diagForm.Text = 'Diagnostyka drukarki - Etykiety IT v2.4'
    $diagForm.StartPosition = 'CenterParent'
    $diagForm.ClientSize = [System.Drawing.Size]::new(900, 690)
    $diagForm.MinimumSize = [System.Drawing.Size]::new(800, 600)
    $diagForm.Font = [System.Drawing.Font]::new('Segoe UI', 9.0)

    $info = [System.Windows.Forms.Label]::new()
    $info.Text = 'Raport poniżej pokazuje wartości zwracane przez sterownik. "Wydruk testowy" doda dane z faktycznego zdarzenia PrintPage.'
    $info.AutoSize = $true
    $info.Location = [System.Drawing.Point]::new(16, 15)
    $diagForm.Controls.Add($info)

    $txt = [System.Windows.Forms.TextBox]::new()
    $txt.Multiline = $true
    $txt.ScrollBars = [System.Windows.Forms.ScrollBars]::Both
    $txt.WordWrap = $false
    $txt.ReadOnly = $true
    $txt.Font = [System.Drawing.Font]::new('Consolas', 9.0)
    $txt.Location = [System.Drawing.Point]::new(16, 45)
    $txt.Size = [System.Drawing.Size]::new(868, 565)
    $txt.Anchor = `
        [System.Windows.Forms.AnchorStyles]::Top -bor `
        [System.Windows.Forms.AnchorStyles]::Bottom -bor `
        [System.Windows.Forms.AnchorStyles]::Left -bor `
        [System.Windows.Forms.AnchorStyles]::Right
    $diagForm.Controls.Add($txt)

    $baseReport = Get-StaticDiagnosticReport `
        -PrinterName $PrinterName `
        -WidthMm $WidthMm `
        -HeightMm $HeightMm `
        -ProfileName $ProfileName `
        -Columns $Columns `
        -Rows $Rows

    $txt.Text = $baseReport

    $btnTest = [System.Windows.Forms.Button]::new()
    $btnTest.Text = 'Wydruk testowy'
    $btnTest.Location = [System.Drawing.Point]::new(16, 625)
    $btnTest.Size = [System.Drawing.Size]::new(150, 38)
    $btnTest.Anchor = `
        [System.Windows.Forms.AnchorStyles]::Bottom -bor `
        [System.Windows.Forms.AnchorStyles]::Left
    $diagForm.Controls.Add($btnTest)

    $btnSave = [System.Windows.Forms.Button]::new()
    $btnSave.Text = 'Zapisz raport'
    $btnSave.Location = [System.Drawing.Point]::new(176, 625)
    $btnSave.Size = [System.Drawing.Size]::new(150, 38)
    $btnSave.Anchor = `
        [System.Windows.Forms.AnchorStyles]::Bottom -bor `
        [System.Windows.Forms.AnchorStyles]::Left
    $diagForm.Controls.Add($btnSave)

    $btnCopy = [System.Windows.Forms.Button]::new()
    $btnCopy.Text = 'Kopiuj'
    $btnCopy.Location = [System.Drawing.Point]::new(336, 625)
    $btnCopy.Size = [System.Drawing.Size]::new(120, 38)
    $btnCopy.Anchor = `
        [System.Windows.Forms.AnchorStyles]::Bottom -bor `
        [System.Windows.Forms.AnchorStyles]::Left
    $diagForm.Controls.Add($btnCopy)

    $btnClose = [System.Windows.Forms.Button]::new()
    $btnClose.Text = 'Zamknij'
    $btnClose.Location = [System.Drawing.Point]::new(764, 625)
    $btnClose.Size = [System.Drawing.Size]::new(120, 38)
    $btnClose.Anchor = `
        [System.Windows.Forms.AnchorStyles]::Bottom -bor `
        [System.Windows.Forms.AnchorStyles]::Right
    $diagForm.Controls.Add($btnClose)

    $btnSave.add_Click({
        try {
            if (-not (Test-Path $DiagnosticsDir)) {
                New-Item -ItemType Directory -Path $DiagnosticsDir -Force | Out-Null
            }

            $safePrinter = ($PrinterName -replace '[\\/:*?"<>|]', '_')
            $file = Join-Path $DiagnosticsDir (
                'diag_{0}_{1}.txt' -f $safePrinter, (Get-Date).ToString('yyyyMMdd_HHmmss')
            )
            $txt.Text | Set-Content -LiteralPath $file -Encoding UTF8

            [void][System.Windows.Forms.MessageBox]::Show(
                $diagForm,
                "Raport zapisano:`r`n$file",
                'Diagnostyka',
                [System.Windows.Forms.MessageBoxButtons]::OK,
                [System.Windows.Forms.MessageBoxIcon]::Information
            )
        }
        catch {
            [void][System.Windows.Forms.MessageBox]::Show(
                $diagForm,
                $_.Exception.Message,
                'Błąd zapisu',
                [System.Windows.Forms.MessageBoxButtons]::OK,
                [System.Windows.Forms.MessageBoxIcon]::Error
            )
        }
    })

    $btnCopy.add_Click({
        try {
            [System.Windows.Forms.Clipboard]::SetText($txt.Text)
        }
        catch {}
    })

    $btnClose.add_Click({
        $diagForm.Close()
    })

    $btnTest.add_Click({
        $answer = [System.Windows.Forms.MessageBox]::Show(
            $diagForm,
            "Wydrukować jedną etykietę diagnostyczną na drukarce:`r`n$PrinterName ?",
            'Wydruk diagnostyczny',
            [System.Windows.Forms.MessageBoxButtons]::YesNo,
            [System.Windows.Forms.MessageBoxIcon]::Question
        )

        if ($answer -ne [System.Windows.Forms.DialogResult]::Yes) { return }

        $pkg = $null
        try {
            $pkg = Get-DiagnosticPrintDocument `
                -PrinterName $PrinterName `
                -WidthMm $WidthMm `
                -HeightMm $HeightMm

            $doc = $pkg.Document
            $runtimeLines = [System.Collections.Generic.List[string]]::new()

            $doc.add_PrintPage({
                param($sender, $e)

                $g = $e.Graphics

                # Runtime diagnostic
                $runtimeLines.Add('')
                $runtimeLines.Add('=== RUNTIME: PrintPage ===')
                $runtimeLines.Add(('PageBounds: {0}' -f (Format-RectDiagnostic $e.PageBounds)))
                $runtimeLines.Add(('MarginBounds: {0}' -f (Format-RectDiagnostic $e.MarginBounds)))
                $runtimeLines.Add(('PageSettings.Bounds: {0}' -f (Format-RectDiagnostic $e.PageSettings.Bounds)))
                $runtimeLines.Add(('PageSettings.PrintableArea: {0}' -f (Format-RectDiagnostic $e.PageSettings.PrintableArea)))
                $runtimeLines.Add(('HardMarginX/Y: {0:0.##} / {1:0.##} [1/100 in] = {2:0.00} / {3:0.00} mm' -f `
                    $e.PageSettings.HardMarginX,
                    $e.PageSettings.HardMarginY,
                    (Convert-HundredthsInchToMm $e.PageSettings.HardMarginX),
                    (Convert-HundredthsInchToMm $e.PageSettings.HardMarginY)))
                $runtimeLines.Add(('Landscape: {0}' -f $e.PageSettings.Landscape))
                $runtimeLines.Add(('Graphics DpiX/DpiY: {0:0.##} / {1:0.##}' -f $g.DpiX, $g.DpiY))
                $runtimeLines.Add(('Graphics PageUnit BEFORE: {0}' -f $g.PageUnit))
                $runtimeLines.Add(('Graphics VisibleClipBounds BEFORE: X={0:0.##}, Y={1:0.##}, W={2:0.##}, H={3:0.##}' -f `
                    $g.VisibleClipBounds.X, $g.VisibleClipBounds.Y, $g.VisibleClipBounds.Width, $g.VisibleClipBounds.Height))

                # Test rysowany w mm bez żadnej kompensacji.
                $g.PageUnit = [System.Drawing.GraphicsUnit]::Millimeter
                $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality

                $runtimeLines.Add(('Graphics PageUnit AFTER: {0}' -f $g.PageUnit))
                $runtimeLines.Add(('Graphics VisibleClipBounds AFTER [mm]: X={0:0.00}, Y={1:0.00}, W={2:0.00}, H={3:0.00}' -f `
                    $g.VisibleClipBounds.X, $g.VisibleClipBounds.Y, $g.VisibleClipBounds.Width, $g.VisibleClipBounds.Height))

                $penThin = [System.Drawing.Pen]::new([System.Drawing.Color]::Black, 0.2)
                $penCenter = [System.Drawing.Pen]::new([System.Drawing.Color]::Black, 0.7)
                $penCenter.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash
                $fontSmall = [System.Drawing.Font]::new('Arial', 5.5, [System.Drawing.FontStyle]::Regular)
                $fontCenter = [System.Drawing.Font]::new('Arial', 7.0, [System.Drawing.FontStyle]::Bold)

                try {
                    # Linie co 5 mm w osi X.
                    for ($x = 0; $x -le $WidthMm; $x += 5) {
                        [single]$len = if (($x % 10) -eq 0) { 5.0 } else { 3.0 }
                        $g.DrawLine($penThin, [single]$x, 1.0, [single]$x, [single](1.0 + $len))

                        if (($x % 10) -eq 0) {
                            $g.DrawString(
                                ([string][int]$x),
                                $fontSmall,
                                [System.Drawing.Brushes]::Black,
                                [single]($x + 0.5),
                                1.5
                            )
                        }
                    }

                    # Linie pionowe kontrolne: 0, 1/4, 1/2, 3/4, 100%.
                    [single]$q1 = $WidthMm * 0.25
                    [single]$mid = $WidthMm * 0.50
                    [single]$q3 = $WidthMm * 0.75

                    $g.DrawLine($penThin, $q1, 8.0, $q1, [single]($HeightMm - 4.0))
                    $g.DrawLine($penCenter, $mid, 7.0, $mid, [single]($HeightMm - 2.0))
                    $g.DrawLine($penThin, $q3, 8.0, $q3, [single]($HeightMm - 4.0))

                    # Oczekiwany środek
                    $midTextRect = [System.Drawing.RectangleF]::new(
                        [single]($mid - 13.0),
                        [single]($HeightMm * 0.43),
                        26.0,
                        8.0
                    )
                    $sf = [System.Drawing.StringFormat]::new()
                    $sf.Alignment = [System.Drawing.StringAlignment]::Center
                    $sf.LineAlignment = [System.Drawing.StringAlignment]::Center
                    try {
                        $g.DrawString(
                            ('ŚRODEK {0:0.0} mm' -f $mid),
                            $fontCenter,
                            [System.Drawing.Brushes]::Black,
                            $midTextRect,
                            $sf
                        )
                    }
                    finally {
                        $sf.Dispose()
                    }

                    # Ramka żądanego fizycznego formatu - może zostać przycięta
                    # i właśnie to jest informacją diagnostyczną.
                    $g.DrawRectangle(
                        $penThin,
                        0.2,
                        0.2,
                        [single]($WidthMm - 0.4),
                        [single]($HeightMm - 0.4)
                    )

                    $g.DrawString(
                        ('TEST {0:0.0} x {1:0.0} mm' -f $WidthMm, $HeightMm),
                        $fontSmall,
                        [System.Drawing.Brushes]::Black,
                        3.0,
                        [single]($HeightMm - 7.0)
                    )
                }
                finally {
                    $penThin.Dispose()
                    $penCenter.Dispose()
                    $fontSmall.Dispose()
                    $fontCenter.Dispose()
                }

                $e.HasMorePages = $false
            }.GetNewClosure())

            $doc.Print()

            # Po Print() zdarzenie PrintPage jest synchronicznie zakończone,
            # więc runtimeLines są już uzupełnione.
            $combined = $baseReport + "`r`n" + ($runtimeLines -join "`r`n")
            $txt.Text = $combined

            if (-not (Test-Path $DiagnosticsDir)) {
                New-Item -ItemType Directory -Path $DiagnosticsDir -Force | Out-Null
            }

            $safePrinter = ($PrinterName -replace '[\\/:*?"<>|]', '_')
            $file = Join-Path $DiagnosticsDir (
                'diag_print_{0}_{1}.txt' -f $safePrinter, (Get-Date).ToString('yyyyMMdd_HHmmss')
            )
            $combined | Set-Content -LiteralPath $file -Encoding UTF8

            [void][System.Windows.Forms.MessageBox]::Show(
                $diagForm,
                "Wydruk diagnostyczny wysłano.`r`n`r`nRaport zapisano:`r`n$file`r`n`r`nPo wydruku zmierz położenie przerywanej linii względem lewej i prawej fizycznej krawędzi etykiety.",
                'Diagnostyka',
                [System.Windows.Forms.MessageBoxButtons]::OK,
                [System.Windows.Forms.MessageBoxIcon]::Information
            )
        }
        catch {
            [void][System.Windows.Forms.MessageBox]::Show(
                $diagForm,
                "Błąd wydruku diagnostycznego:`r`n`r`n$($_.Exception.Message)",
                'Diagnostyka',
                [System.Windows.Forms.MessageBoxButtons]::OK,
                [System.Windows.Forms.MessageBoxIcon]::Error
            )
        }
        finally {
            if ($pkg -and $pkg.Document) {
                try { $pkg.Document.Dispose() } catch {}
            }
        }
    })

    [void]$diagForm.ShowDialog($Owner)
    $diagForm.Dispose()
}

# ---------------------------------------------------------
# GUI
# ---------------------------------------------------------
$settings = Load-Settings
$printers = @(Get-InstalledPrinters)

$form = [System.Windows.Forms.Form]::new()
$form.Text = 'Etykiety IT v2.4'
$form.StartPosition = 'CenterScreen'
$form.ClientSize = [System.Drawing.Size]::new(760, 820)
$form.MinimumSize = [System.Drawing.Size]::new(776, 859)
$form.Font = [System.Drawing.Font]::new('Segoe UI', 9.0)
$form.MaximizeBox = $false

$title = [System.Windows.Forms.Label]::new()
$title.Text = 'Etykiety inwentarzowe IT'
$title.Font = [System.Drawing.Font]::new('Segoe UI Semibold', 18.0)
$title.AutoSize = $true
$title.Location = [System.Drawing.Point]::new(24, 18)
$form.Controls.Add($title)

$subtitle = [System.Windows.Forms.Label]::new()
$subtitle.Text = 'Profile rozmiarów • własny format • automatyczna numeracja'
$subtitle.ForeColor = [System.Drawing.Color]::DimGray
$subtitle.AutoSize = $true
$subtitle.Location = [System.Drawing.Point]::new(27, 55)
$form.Controls.Add($subtitle)

# ---------------------------------------------------------
# Grupa: drukarka
# ---------------------------------------------------------
$grpPrinter = [System.Windows.Forms.GroupBox]::new()
$grpPrinter.Text = 'Drukarka'
$grpPrinter.Location = [System.Drawing.Point]::new(24, 88)
$grpPrinter.Size = [System.Drawing.Size]::new(712, 78)
$form.Controls.Add($grpPrinter)

$lblPrinter = [System.Windows.Forms.Label]::new()
$lblPrinter.Text = 'Drukarka:'
$lblPrinter.AutoSize = $true
$lblPrinter.Location = [System.Drawing.Point]::new(18, 34)
$grpPrinter.Controls.Add($lblPrinter)

$cmbPrinter = [System.Windows.Forms.ComboBox]::new()
$cmbPrinter.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDownList
$cmbPrinter.Location = [System.Drawing.Point]::new(115, 29)
$cmbPrinter.Size = [System.Drawing.Size]::new(460, 27)
$grpPrinter.Controls.Add($cmbPrinter)

$btnRefresh = [System.Windows.Forms.Button]::new()
$btnRefresh.Text = 'Odśwież'
$btnRefresh.Location = [System.Drawing.Point]::new(587, 28)
$btnRefresh.Size = [System.Drawing.Size]::new(100, 29)
$grpPrinter.Controls.Add($btnRefresh)

foreach ($p in $printers) {
    [void]$cmbPrinter.Items.Add($p)
}

if ($cmbPrinter.Items.Count -gt 0) {
    $preferred = [string]$settings.PrinterName
    if ($preferred -and $cmbPrinter.Items.Contains($preferred)) {
        $cmbPrinter.SelectedItem = $preferred
    }
    else {
        $dymoIndex = -1
        for ($i = 0; $i -lt $cmbPrinter.Items.Count; $i++) {
            if ([string]$cmbPrinter.Items[$i] -match 'DYMO') {
                $dymoIndex = $i
                break
            }
        }
        $cmbPrinter.SelectedIndex = if ($dymoIndex -ge 0) { $dymoIndex } else { 0 }
    }
}

# ---------------------------------------------------------
# Grupa: format
# ---------------------------------------------------------
$grpFormat = [System.Windows.Forms.GroupBox]::new()
$grpFormat.Text = 'Format etykiety'
$grpFormat.Location = [System.Drawing.Point]::new(24, 178)
$grpFormat.Size = [System.Drawing.Size]::new(712, 194)
$form.Controls.Add($grpFormat)

$lblProfile = [System.Windows.Forms.Label]::new()
$lblProfile.Text = 'Profil:'
$lblProfile.AutoSize = $true
$lblProfile.Location = [System.Drawing.Point]::new(18, 34)
$grpFormat.Controls.Add($lblProfile)

$cmbProfile = [System.Windows.Forms.ComboBox]::new()
$cmbProfile.DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDownList
$cmbProfile.Location = [System.Drawing.Point]::new(115, 29)
$cmbProfile.Size = [System.Drawing.Size]::new(572, 27)
$grpFormat.Controls.Add($cmbProfile)

foreach ($p in $BuiltInProfiles) {
    [void]$cmbProfile.Items.Add($p.Name)
}

$lblWidth = [System.Windows.Forms.Label]::new()
$lblWidth.Text = 'Szerokość [mm]:'
$lblWidth.AutoSize = $true
$lblWidth.Location = [System.Drawing.Point]::new(18, 79)
$grpFormat.Controls.Add($lblWidth)

$numWidth = [System.Windows.Forms.NumericUpDown]::new()
$numWidth.DecimalPlaces = 1
$numWidth.Increment = [decimal]0.5
$numWidth.Minimum = 20
$numWidth.Maximum = 200
$numWidth.Value = [decimal]89.0
$numWidth.Location = [System.Drawing.Point]::new(135, 75)
$numWidth.Size = [System.Drawing.Size]::new(90, 27)
$grpFormat.Controls.Add($numWidth)

$lblHeight = [System.Windows.Forms.Label]::new()
$lblHeight.Text = 'Wysokość [mm]:'
$lblHeight.AutoSize = $true
$lblHeight.Location = [System.Drawing.Point]::new(255, 79)
$grpFormat.Controls.Add($lblHeight)

$numHeight = [System.Windows.Forms.NumericUpDown]::new()
$numHeight.DecimalPlaces = 1
$numHeight.Increment = [decimal]0.5
$numHeight.Minimum = 15
$numHeight.Maximum = 200
$numHeight.Value = [decimal]41.0
$numHeight.Location = [System.Drawing.Point]::new(370, 75)
$numHeight.Size = [System.Drawing.Size]::new(90, 27)
$grpFormat.Controls.Add($numHeight)

$lblColumns = [System.Windows.Forms.Label]::new()
$lblColumns.Text = 'Kolumny:'
$lblColumns.AutoSize = $true
$lblColumns.Location = [System.Drawing.Point]::new(492, 79)
$grpFormat.Controls.Add($lblColumns)

$numColumns = [System.Windows.Forms.NumericUpDown]::new()
$numColumns.Minimum = 1
$numColumns.Maximum = 4
$numColumns.Value = 2
$numColumns.Location = [System.Drawing.Point]::new(555, 75)
$numColumns.Size = [System.Drawing.Size]::new(55, 27)
$grpFormat.Controls.Add($numColumns)

$lblRows = [System.Windows.Forms.Label]::new()
$lblRows.Text = 'Wiersze:'
$lblRows.AutoSize = $true
$lblRows.Location = [System.Drawing.Point]::new(18, 121)
$grpFormat.Controls.Add($lblRows)

$numRows = [System.Windows.Forms.NumericUpDown]::new()
$numRows.Minimum = 1
$numRows.Maximum = 4
$numRows.Value = 1
$numRows.Location = [System.Drawing.Point]::new(115, 117)
$numRows.Size = [System.Drawing.Size]::new(55, 27)
$grpFormat.Controls.Add($numRows)

$chkCutLines = [System.Windows.Forms.CheckBox]::new()
$chkCutLines.Text = 'Drukuj linie cięcia między małymi etykietami'
$chkCutLines.Checked = [bool]$settings.DrawCutLines
$chkCutLines.AutoSize = $true
$chkCutLines.Location = [System.Drawing.Point]::new(255, 120)
$grpFormat.Controls.Add($chkCutLines)

$formatInfo = [System.Windows.Forms.Label]::new()
$formatInfo.ForeColor = [System.Drawing.Color]::DimGray
$formatInfo.AutoSize = $true
$formatInfo.Location = [System.Drawing.Point]::new(18, 157)
$grpFormat.Controls.Add($formatInfo)

# ---------------------------------------------------------
# Grupa: numeracja
# ---------------------------------------------------------
$grpNumbers = [System.Windows.Forms.GroupBox]::new()
$grpNumbers.Text = 'Numeracja'
$grpNumbers.Location = [System.Drawing.Point]::new(24, 385)
$grpNumbers.Size = [System.Drawing.Size]::new(712, 142)
$form.Controls.Add($grpNumbers)

$lblStart = [System.Windows.Forms.Label]::new()
$lblStart.Text = 'Pierwszy numer:'
$lblStart.AutoSize = $true
$lblStart.Location = [System.Drawing.Point]::new(18, 37)
$grpNumbers.Controls.Add($lblStart)

$prefixLabel = [System.Windows.Forms.Label]::new()
$prefixLabel.Text = $Prefix
$prefixLabel.Font = [System.Drawing.Font]::new('Consolas', 10.5, [System.Drawing.FontStyle]::Bold)
$prefixLabel.AutoSize = $true
$prefixLabel.Location = [System.Drawing.Point]::new(135, 35)
$grpNumbers.Controls.Add($prefixLabel)

$numStart = [System.Windows.Forms.NumericUpDown]::new()
$numStart.Minimum = 0
$numStart.Maximum = 999999
$numStart.Value = [decimal][Math]::Max(0, [int]$settings.NextNumber)
$numStart.Location = [System.Drawing.Point]::new(167, 33)
$numStart.Size = [System.Drawing.Size]::new(126, 27)
$numStart.Font = [System.Drawing.Font]::new('Consolas', 10.5)
$grpNumbers.Controls.Add($numStart)

$lblQuantity = [System.Windows.Forms.Label]::new()
$lblQuantity.Text = 'Liczba małych etykiet:'
$lblQuantity.AutoSize = $true
$lblQuantity.Location = [System.Drawing.Point]::new(338, 37)
$grpNumbers.Controls.Add($lblQuantity)

$numQuantity = [System.Windows.Forms.NumericUpDown]::new()
$numQuantity.Minimum = 1
$numQuantity.Maximum = 1000
$numQuantity.Value = 2
$numQuantity.Location = [System.Drawing.Point]::new(515, 33)
$numQuantity.Size = [System.Drawing.Size]::new(125, 27)
$grpNumbers.Controls.Add($numQuantity)

$chkAdvance = [System.Windows.Forms.CheckBox]::new()
$chkAdvance.Text = 'Po wydruku ustaw następny wolny numer'
$chkAdvance.Checked = $true
$chkAdvance.AutoSize = $true
$chkAdvance.Location = [System.Drawing.Point]::new(135, 75)
$grpNumbers.Controls.Add($chkAdvance)

$rangeBox = [System.Windows.Forms.Panel]::new()
$rangeBox.BorderStyle = [System.Windows.Forms.BorderStyle]::FixedSingle
$rangeBox.Location = [System.Drawing.Point]::new(18, 101)
$rangeBox.Size = [System.Drawing.Size]::new(669, 28)
$grpNumbers.Controls.Add($rangeBox)

$lblRange = [System.Windows.Forms.Label]::new()
$lblRange.AutoSize = $true
$lblRange.Font = [System.Drawing.Font]::new('Segoe UI Semibold', 9.0)
$lblRange.Location = [System.Drawing.Point]::new(8, 4)
$rangeBox.Controls.Add($lblRange)

# ---------------------------------------------------------
# Przyciski
# ---------------------------------------------------------
$btnPreview = [System.Windows.Forms.Button]::new()
$btnPreview.Text = 'Podgląd'
$btnPreview.Location = [System.Drawing.Point]::new(24, 544)
$btnPreview.Size = [System.Drawing.Size]::new(155, 42)
$form.Controls.Add($btnPreview)

$btnPrint = [System.Windows.Forms.Button]::new()
$btnPrint.Text = 'DRUKUJ'
$btnPrint.Font = [System.Drawing.Font]::new('Segoe UI Semibold', 10.0)
$btnPrint.Location = [System.Drawing.Point]::new(190, 544)
$btnPrint.Size = [System.Drawing.Size]::new(190, 42)
$form.Controls.Add($btnPrint)

$btnHistory = [System.Windows.Forms.Button]::new()
$btnHistory.Text = 'Otwórz historię'
$btnHistory.Location = [System.Drawing.Point]::new(392, 544)
$btnHistory.Size = [System.Drawing.Size]::new(150, 42)
$form.Controls.Add($btnHistory)

$btnDiagnostics = [System.Windows.Forms.Button]::new()
$btnDiagnostics.Text = 'Diagnostyka'
$btnDiagnostics.Location = [System.Drawing.Point]::new(552, 544)
$btnDiagnostics.Size = [System.Drawing.Size]::new(184, 42)
$form.Controls.Add($btnDiagnostics)


# ---------------------------------------------------------
# Historia
# ---------------------------------------------------------
$historyGroup = [System.Windows.Forms.GroupBox]::new()
$historyGroup.Text = 'Ostatnie wydruki'
$historyGroup.Location = [System.Drawing.Point]::new(24, 600)
$historyGroup.Size = [System.Drawing.Size]::new(712, 190)
$form.Controls.Add($historyGroup)

$listHistory = [System.Windows.Forms.ListView]::new()
$listHistory.View = [System.Windows.Forms.View]::Details
$listHistory.FullRowSelect = $true
$listHistory.GridLines = $true
$listHistory.Location = [System.Drawing.Point]::new(12, 22)
$listHistory.Size = [System.Drawing.Size]::new(688, 154)
[void]$listHistory.Columns.Add('Data', 125)
[void]$listHistory.Columns.Add('Zakres', 205)
[void]$listHistory.Columns.Add('Szt.', 48)
[void]$listHistory.Columns.Add('Format', 180)
[void]$listHistory.Columns.Add('Drukarka', 128)
$historyGroup.Controls.Add($listHistory)

$status = [System.Windows.Forms.StatusStrip]::new()
$statusLabel = [System.Windows.Forms.ToolStripStatusLabel]::new()
$statusLabel.Text = 'Gotowe'
[void]$status.Items.Add($statusLabel)
$form.Controls.Add($status)

# ---------------------------------------------------------
# Logika GUI
# ---------------------------------------------------------
$script:LoadingProfile = $false

function Get-CurrentPrinter {
    if ($cmbPrinter.SelectedItem) {
        return [string]$cmbPrinter.SelectedItem
    }
    return ''
}

function Get-CurrentProfileName {
    if ($cmbProfile.SelectedItem) {
        return [string]$cmbProfile.SelectedItem
    }
    return 'Własny rozmiar'
}

function Get-SlotsPerPhysicalLabel {
    return ([int]$numColumns.Value * [int]$numRows.Value)
}

function Update-FormatInfo {
    $w = [double]$numWidth.Value
    $h = [double]$numHeight.Value
    $cols = [int]$numColumns.Value
    $rows = [int]$numRows.Value

    $smallW = $w / $cols
    $smallH = $h / $rows
    $formatInfo.Text =
        "Mała etykieta: {0:0.0} × {1:0.0} mm   |   sztuk na jednej fizycznej: {2}" -f `
        $smallW, $smallH, ($cols * $rows)
}

function Update-RangeLabel {
    $start = [int]$numStart.Value
    $qty = [int]$numQuantity.Value
    $end = $start + $qty - 1
    $slots = Get-SlotsPerPhysicalLabel
    $physical = [int][Math]::Ceiling($qty / [double]$slots)

    $lblRange.Text =
        "Zakres: $(Format-AssetId $start) – $(Format-AssetId $end)   |   fizycznych etykiet: $physical"
}

function Refresh-HistoryView {
    $listHistory.Items.Clear()

    $rows = @(Get-History | Select-Object -Last 8)
    [array]::Reverse($rows)

    foreach ($r in $rows) {
        $range = "$($r.FirstId) – $($r.LastId)"
        $item = [System.Windows.Forms.ListViewItem]::new([string]$r.Timestamp)
        [void]$item.SubItems.Add($range)
        [void]$item.SubItems.Add([string]$r.Quantity)
        [void]$item.SubItems.Add([string]$r.Profile)
        [void]$item.SubItems.Add([string]$r.Printer)
        [void]$listHistory.Items.Add($item)
    }
}

function Save-CurrentSettings {
    Save-Settings `
        -NextNumber ([int]$numStart.Value) `
        -PrinterName (Get-CurrentPrinter) `
        -ProfileName (Get-CurrentProfileName) `
        -CustomWidthMm ([double]$numWidth.Value) `
        -CustomHeightMm ([double]$numHeight.Value) `
        -CustomColumns ([int]$numColumns.Value) `
        -CustomRows ([int]$numRows.Value) `
        -DrawCutLines ([bool]$chkCutLines.Checked)
}

function Apply-SelectedProfile {
    if ($script:LoadingProfile) { return }
    $script:LoadingProfile = $true
    try {
        $name = Get-CurrentProfileName
        $profile = $BuiltInProfiles | Where-Object Name -eq $name | Select-Object -First 1

        if ($name -eq 'Własny rozmiar') {
            $numWidth.Enabled = $true
            $numHeight.Enabled = $true
            $numColumns.Enabled = $true
            $numRows.Enabled = $true

            $numWidth.Value = [decimal][double]$settings.CustomWidthMm
            $numHeight.Value = [decimal][double]$settings.CustomHeightMm
            $numColumns.Value = [decimal][int]$settings.CustomColumns
            $numRows.Value = [decimal][int]$settings.CustomRows
        }
        elseif ($profile) {
            $numWidth.Value = [decimal][double]$profile.WidthMm
            $numHeight.Value = [decimal][double]$profile.HeightMm
            $numColumns.Value = [decimal][int]$profile.Columns
            $numRows.Value = [decimal][int]$profile.Rows

            # Wbudowane profile można oglądać, ale nie zmieniać przypadkiem.
            $numWidth.Enabled = $false
            $numHeight.Enabled = $false
            $numColumns.Enabled = $false
            $numRows.Enabled = $false
        }

        Update-FormatInfo
        Update-RangeLabel
    }
    finally {
        $script:LoadingProfile = $false
    }
}

function Set-Busy {
    param([bool]$Busy, [string]$Text = '')
    $btnPreview.Enabled = -not $Busy
    $btnPrint.Enabled = -not $Busy
    $btnHistory.Enabled = -not $Busy
    $btnDiagnostics.Enabled = -not $Busy
    $btnRefresh.Enabled = -not $Busy
    $cmbPrinter.Enabled = -not $Busy
    $cmbProfile.Enabled = -not $Busy
    $numStart.Enabled = -not $Busy
    $numQuantity.Enabled = -not $Busy
    $chkAdvance.Enabled = -not $Busy
    $chkCutLines.Enabled = -not $Busy

    # Przywrócenie edytowalności pól formatu nastąpi przez profil.
    if (-not $Busy) {
        Apply-SelectedProfile
    }

    if ($Text) { $statusLabel.Text = $Text }
    [System.Windows.Forms.Application]::DoEvents()
}

function Show-Error {
    param([string]$Message)
    [void][System.Windows.Forms.MessageBox]::Show(
        $form,
        $Message,
        'Etykiety IT',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error
    )
}

function Validate-Printer {
    if (-not (Get-CurrentPrinter)) {
        Show-Error 'Wybierz drukarkę.'
        return $false
    }
    return $true
}

# Wybór profilu z poprzedniej sesji.
$profileIndex = $cmbProfile.Items.IndexOf([string]$settings.ProfileName)
if ($profileIndex -lt 0) { $profileIndex = 0 }
$cmbProfile.SelectedIndex = $profileIndex

$cmbProfile.add_SelectedIndexChanged({
    Apply-SelectedProfile
    try { Save-CurrentSettings } catch {}
})

$numWidth.add_ValueChanged({
    if ((Get-CurrentProfileName) -eq 'Własny rozmiar' -and -not $script:LoadingProfile) {
        $settings.CustomWidthMm = [double]$numWidth.Value
        Update-FormatInfo
        Update-RangeLabel
    }
})
$numHeight.add_ValueChanged({
    if ((Get-CurrentProfileName) -eq 'Własny rozmiar' -and -not $script:LoadingProfile) {
        $settings.CustomHeightMm = [double]$numHeight.Value
        Update-FormatInfo
        Update-RangeLabel
    }
})
$numColumns.add_ValueChanged({
    if ((Get-CurrentProfileName) -eq 'Własny rozmiar' -and -not $script:LoadingProfile) {
        $settings.CustomColumns = [int]$numColumns.Value
        Update-FormatInfo
        Update-RangeLabel
    }
})
$numRows.add_ValueChanged({
    if ((Get-CurrentProfileName) -eq 'Własny rozmiar' -and -not $script:LoadingProfile) {
        $settings.CustomRows = [int]$numRows.Value
        Update-FormatInfo
        Update-RangeLabel
    }
})

$numStart.add_ValueChanged({ Update-RangeLabel })
$numQuantity.add_ValueChanged({ Update-RangeLabel })

$btnRefresh.add_Click({
    try {
        $current = Get-CurrentPrinter
        $cmbPrinter.Items.Clear()
        $fresh = @(Get-InstalledPrinters)
        foreach ($p in $fresh) { [void]$cmbPrinter.Items.Add($p) }

        if ($current -and $cmbPrinter.Items.Contains($current)) {
            $cmbPrinter.SelectedItem = $current
        }
        elseif ($cmbPrinter.Items.Count -gt 0) {
            $dymoIndex = -1
            for ($i = 0; $i -lt $cmbPrinter.Items.Count; $i++) {
                if ([string]$cmbPrinter.Items[$i] -match 'DYMO') {
                    $dymoIndex = $i
                    break
                }
            }
            $cmbPrinter.SelectedIndex = if ($dymoIndex -ge 0) { $dymoIndex } else { 0 }
        }

        $statusLabel.Text = "Odświeżono listę drukarek ($($fresh.Count))."
    }
    catch {
        Show-Error $_.Exception.Message
    }
})

$btnDiagnostics.add_Click({
    if (-not (Validate-Printer)) { return }

    try {
        Show-DiagnosticWindow `
            -PrinterName (Get-CurrentPrinter) `
            -WidthMm ([double]$numWidth.Value) `
            -HeightMm ([double]$numHeight.Value) `
            -ProfileName (Get-CurrentProfileName) `
            -Columns ([int]$numColumns.Value) `
            -Rows ([int]$numRows.Value) `
            -Owner $form
    }
    catch {
        Show-Error "Nie udało się uruchomić diagnostyki.`r`n`r`n$($_.Exception.Message)"
    }
})

$btnHistory.add_Click({
    try {
        Ensure-HistoryFile
        Start-Process -FilePath $HistoryPath
    }
    catch {
        Show-Error "Nie udało się otworzyć historii.`r`n`r`n$($_.Exception.Message)"
    }
})

$btnPreview.add_Click({
    if (-not (Validate-Printer)) { return }

    $printer = Get-CurrentPrinter
    $start = [int]$numStart.Value
    $qty = [int]$numQuantity.Value
    $w = [double]$numWidth.Value
    $h = [double]$numHeight.Value
    $cols = [int]$numColumns.Value
    $rows = [int]$numRows.Value

    $pkg = $null
    try {
        Set-Busy $true 'Generowanie podglądu...'

        $pkg = New-LabelPrintDocument `
            -PrinterName $printer `
            -StartNumber $start `
            -Quantity $qty `
            -WidthMm $w `
            -HeightMm $h `
            -Columns $cols `
            -Rows $rows `
            -DrawCutLines ([bool]$chkCutLines.Checked)

        $preview = [System.Windows.Forms.PrintPreviewDialog]::new()
        $preview.Document = $pkg.Document
        $preview.Width = 1100
        $preview.Height = 720
        $preview.StartPosition = 'CenterParent'
        [void]$preview.ShowDialog($form)
        $preview.Dispose()

        $statusLabel.Text = 'Podgląd zamknięty. Numery nie zostały zarezerwowane.'
    }
    catch {
        Show-Error "Nie udało się wygenerować podglądu.`r`n`r`n$($_.Exception.Message)"
        $statusLabel.Text = 'Błąd podglądu.'
    }
    finally {
        Dispose-PrintPackage $pkg
        Set-Busy $false
    }
})

$btnPrint.add_Click({
    if (-not (Validate-Printer)) { return }

    $printer = Get-CurrentPrinter
    $start = [int]$numStart.Value
    $qty = [int]$numQuantity.Value
    $end = $start + $qty - 1

    $w = [double]$numWidth.Value
    $h = [double]$numHeight.Value
    $cols = [int]$numColumns.Value
    $rows = [int]$numRows.Value
    $slots = $cols * $rows
    $physical = [int][Math]::Ceiling($qty / [double]$slots)
    $profile = Get-CurrentProfileName

    $overlap = Find-Overlap -StartNumber $start -EndNumber $end
    if ($overlap) {
        $msg =
            "Uwaga: wybrany zakres nachodzi na wcześniej drukowane numery.`r`n`r`n" +
            "Poprzedni wydruk: $($overlap.FirstId) – $($overlap.LastId)`r`n" +
            "Data: $($overlap.Timestamp)`r`n`r`n" +
            "Czy mimo to wydrukować ponownie?"

        $answer = [System.Windows.Forms.MessageBox]::Show(
            $form,
            $msg,
            'Możliwy duplikat',
            [System.Windows.Forms.MessageBoxButtons]::YesNo,
            [System.Windows.Forms.MessageBoxIcon]::Warning
        )

        if ($answer -ne [System.Windows.Forms.DialogResult]::Yes) {
            $statusLabel.Text = 'Drukowanie anulowane — wykryto możliwy duplikat.'
            return
        }
    }

    $confirm =
        "Drukarka: $printer`r`n" +
        "Profil: $profile`r`n" +
        "Fizyczny rozmiar: $w × $h mm`r`n" +
        "Układ: $cols × $rows ($slots szt. na etykiecie)`r`n" +
        "Zakres: $(Format-AssetId $start) – $(Format-AssetId $end)`r`n" +
        "Małych etykiet: $qty`r`n" +
        "Fizycznych etykiet: $physical`r`n`r`n" +
        "Wysłać do drukarki?"

    $answer = [System.Windows.Forms.MessageBox]::Show(
        $form,
        $confirm,
        'Potwierdzenie wydruku',
        [System.Windows.Forms.MessageBoxButtons]::YesNo,
        [System.Windows.Forms.MessageBoxIcon]::Question
    )
    if ($answer -ne [System.Windows.Forms.DialogResult]::Yes) { return }

    $pkg = $null
    try {
        Set-Busy $true 'Wysyłanie etykiet do drukarki...'

        $pkg = New-LabelPrintDocument `
            -PrinterName $printer `
            -StartNumber $start `
            -Quantity $qty `
            -WidthMm $w `
            -HeightMm $h `
            -Columns $cols `
            -Rows $rows `
            -DrawCutLines ([bool]$chkCutLines.Checked)

        $pkg.Document.Print()

        Add-History `
            -StartNumber $start `
            -EndNumber $end `
            -Quantity $qty `
            -PhysicalLabels $physical `
            -Printer $printer `
            -Profile $profile `
            -WidthMm $w `
            -HeightMm $h `
            -Columns $cols `
            -Rows $rows

        $next = $end + 1
        if ($chkAdvance.Checked) {
            $numStart.Value = [decimal]$next
        }

        Save-Settings `
            -NextNumber $next `
            -PrinterName $printer `
            -ProfileName $profile `
            -CustomWidthMm ([double]$settings.CustomWidthMm) `
            -CustomHeightMm ([double]$settings.CustomHeightMm) `
            -CustomColumns ([int]$settings.CustomColumns) `
            -CustomRows ([int]$settings.CustomRows) `
            -DrawCutLines ([bool]$chkCutLines.Checked)

        Refresh-HistoryView
        Update-RangeLabel

        $statusLabel.Text =
            "Wysłano: $(Format-AssetId $start) – $(Format-AssetId $end). Następny: $(Format-AssetId $next)."
    }
    catch {
        Show-Error "Nie udało się wydrukować etykiet.`r`n`r`n$($_.Exception.Message)"
        $statusLabel.Text = 'Błąd drukowania. Numeracja nie została przesunięta.'
    }
    finally {
        Dispose-PrintPackage $pkg
        Set-Busy $false
    }
})

$cmbPrinter.add_SelectedIndexChanged({
    try { Save-CurrentSettings } catch {}
})

$chkCutLines.add_CheckedChanged({
    try { Save-CurrentSettings } catch {}
})

$form.add_FormClosing({
    try { Save-CurrentSettings } catch {}
})

Apply-SelectedProfile
Update-FormatInfo
Update-RangeLabel
Refresh-HistoryView

if ($printers.Count -eq 0) {
    $statusLabel.Text = 'Nie znaleziono zainstalowanych drukarek.'
}

try {
    [void]$form.ShowDialog()
}
catch {
    [void][System.Windows.Forms.MessageBox]::Show(
        "Wystąpił błąd aplikacji:`r`n`r`n$($_.Exception.Message)",
        'Etykiety IT',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error
    )
}
finally {
    try { $form.Dispose() } catch {}
}
