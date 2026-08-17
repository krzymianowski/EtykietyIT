[CmdletBinding()]
param(
    [string]$ExpectedVersion
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,

        [Parameter(Mandatory)]
        [string]$Description
    )

    Write-Host "`n==> $Description"
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Polecenie dotnet zakończyło się kodem ${LASTEXITCODE}: $Description"
    }
}

function Copy-DirectoryContents {
    param(
        [Parameter(Mandatory)]
        [string]$Source,

        [Parameter(Mandatory)]
        [string]$Destination
    )

    Get-ChildItem -LiteralPath $Source -Force | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination $Destination -Recurse -Force
    }
}

function Assert-ReleaseContent {
    param(
        [Parameter(Mandatory)]
        [string]$Directory,

        [Parameter(Mandatory)]
        [bool]$IsPortable
    )

    $requiredPaths = @(
        'EtykietyIT.exe',
        'README.md',
        'LICENSE',
        'THIRD-PARTY-NOTICES.md',
        'DOTNET-LICENSE.txt',
        'DOTNET-THIRD-PARTY-NOTICES.txt',
        'Resources\Profiles'
    )

    foreach ($relativePath in $requiredPaths) {
        $fullPath = Join-Path $Directory $relativePath
        if (-not (Test-Path -LiteralPath $fullPath)) {
            throw "Brak wymaganego elementu paczki: $relativePath"
        }
    }

    $portableMarkerPath = Join-Path $Directory 'portable.mode'
    if ($IsPortable -and -not (Test-Path -LiteralPath $portableMarkerPath -PathType Leaf)) {
        throw 'Paczka Portable nie zawiera pliku portable.mode.'
    }

    if (-not $IsPortable -and (Test-Path -LiteralPath $portableMarkerPath)) {
        throw 'Paczka Standard nie może zawierać pliku portable.mode.'
    }
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$solutionPath = Join-Path $repoRoot 'EtykietyIT.slnx'
$projectPath = Join-Path $repoRoot 'EtykietyIT\EtykietyIT.csproj'
$publishProfilePath = Join-Path $repoRoot `
    'EtykietyIT\Properties\PublishProfiles\win-x64-self-contained.pubxml'

[xml]$projectXml = Get-Content -LiteralPath $projectPath -Raw
$versionNode = $projectXml.SelectSingleNode('/Project/PropertyGroup/Version')
if ($null -eq $versionNode) {
    throw 'Projekt nie zawiera właściwości Version.'
}

$version = $versionNode.InnerText.Trim()
$semanticVersionPattern =
    '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)' +
    '(?:-[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?$'
if ([string]::IsNullOrWhiteSpace($version) -or
    $version -notmatch $semanticVersionPattern) {
    throw "Nieprawidłowa wersja projektu: '$version'."
}

if (-not [string]::IsNullOrWhiteSpace($ExpectedVersion) -and
    $ExpectedVersion.Trim() -ne $version) {
    throw "Wersja projektu '$version' nie odpowiada oczekiwanej wersji " +
        "'$($ExpectedVersion.Trim())'."
}

$artifactsRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $repoRoot 'artifacts'))
$releaseRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $artifactsRoot 'release'))
$repoPrefix = $repoRoot.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar) +
    [System.IO.Path]::DirectorySeparatorChar

if (-not $releaseRoot.StartsWith(
        $repoPrefix,
        [StringComparison]::OrdinalIgnoreCase)) {
    throw "Niebezpieczna ścieżka katalogu release: $releaseRoot"
}

if (Test-Path -LiteralPath $releaseRoot) {
    Remove-Item -LiteralPath $releaseRoot -Recurse -Force
}

$publishDirectory = Join-Path $releaseRoot 'publish'
$stagingRoot = Join-Path $releaseRoot 'staging'
$standardStagingDirectory = Join-Path $stagingRoot `
    "EtykietyIT-$version-win-x64"
$portableStagingDirectory = Join-Path $stagingRoot `
    "EtykietyIT-$version-win-x64-portable"

New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null
New-Item -ItemType Directory -Path $standardStagingDirectory -Force | Out-Null
New-Item -ItemType Directory -Path $portableStagingDirectory -Force | Out-Null

Push-Location $repoRoot
try {
    Invoke-DotNet -Description 'Restore rozwiązania' -Arguments @(
        'restore',
        $solutionPath
    )
    Invoke-DotNet -Description 'Build Release' -Arguments @(
        'build',
        $solutionPath,
        '--configuration',
        'Release',
        '--no-restore'
    )
    Invoke-DotNet -Description 'Testy Release' -Arguments @(
        'test',
        $solutionPath,
        '--configuration',
        'Release',
        '--no-build',
        '--no-restore'
    )
    Invoke-DotNet -Description 'Publish win-x64 self-contained' -Arguments @(
        'publish',
        $projectPath,
        '--configuration',
        'Release',
        "-p:PublishProfile=$publishProfilePath",
        '--output',
        $publishDirectory
    )
}
finally {
    Pop-Location
}

$dotnetCommand = Get-Command dotnet -ErrorAction Stop
$dotnetRoot = Split-Path -Parent $dotnetCommand.Source
$dotnetLicensePath = Join-Path $dotnetRoot 'LICENSE.txt'
$dotnetNoticesPath = Join-Path $dotnetRoot 'ThirdPartyNotices.txt'

foreach ($officialFile in @($dotnetLicensePath, $dotnetNoticesPath)) {
    if (-not (Test-Path -LiteralPath $officialFile -PathType Leaf)) {
        throw "Nie znaleziono oficjalnego pliku dystrybucji .NET: $officialFile"
    }
}

foreach ($stagingDirectory in @(
        $standardStagingDirectory,
        $portableStagingDirectory)) {
    Copy-DirectoryContents -Source $publishDirectory -Destination $stagingDirectory
    Copy-Item -LiteralPath (Join-Path $repoRoot 'README.md') `
        -Destination $stagingDirectory -Force
    Copy-Item -LiteralPath (Join-Path $repoRoot 'LICENSE') `
        -Destination $stagingDirectory -Force
    Copy-Item -LiteralPath (Join-Path $repoRoot 'THIRD-PARTY-NOTICES.md') `
        -Destination $stagingDirectory -Force
    Copy-Item -LiteralPath $dotnetLicensePath `
        -Destination (Join-Path $stagingDirectory 'DOTNET-LICENSE.txt') -Force
    Copy-Item -LiteralPath $dotnetNoticesPath `
        -Destination `
        (Join-Path $stagingDirectory 'DOTNET-THIRD-PARTY-NOTICES.txt') -Force
}

[System.IO.File]::WriteAllText(
    (Join-Path $portableStagingDirectory 'portable.mode'),
    [string]::Empty,
    [System.Text.UTF8Encoding]::new($false))

Assert-ReleaseContent -Directory $standardStagingDirectory -IsPortable $false
Assert-ReleaseContent -Directory $portableStagingDirectory -IsPortable $true

$standardZipPath = Join-Path $releaseRoot `
    "EtykietyIT-$version-win-x64.zip"
$portableZipPath = Join-Path $releaseRoot `
    "EtykietyIT-$version-win-x64-portable.zip"

[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $standardStagingDirectory,
    $standardZipPath,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false)
[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $portableStagingDirectory,
    $portableZipPath,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false)

$checksumLines = @($standardZipPath, $portableZipPath) | ForEach-Object {
    $hash = Get-FileHash -LiteralPath $_ -Algorithm SHA256
    "$($hash.Hash.ToLowerInvariant())  $([System.IO.Path]::GetFileName($_))"
}
$checksumPath = Join-Path $releaseRoot 'SHA256SUMS.txt'
[System.IO.File]::WriteAllText(
    $checksumPath,
    [string]::Join("`r`n", $checksumLines) + "`r`n",
    [System.Text.UTF8Encoding]::new($false))

Write-Host "`nUtworzono release candidate ${version}:"
Write-Host "  $standardZipPath"
Write-Host "  $portableZipPath"
Write-Host "  $checksumPath"
