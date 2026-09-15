$ErrorActionPreference = 'Stop'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) ('Irony Mac Package-' + [Guid]::NewGuid().ToString('N'))
$sourceDirectory = Join-Path $testRoot 'payload'
$outputDirectory = Join-Path $testRoot 'output'

try {
    [IO.Directory]::CreateDirectory($sourceDirectory) | Out-Null
    foreach ($fileName in @('IronyModManager', 'IronyModManager.Updater', 'IronyModManager.GameHandler', 'libhostfxr.dylib', 'IronyModManager.dll')) {
        [IO.File]::WriteAllText((Join-Path $sourceDirectory $fileName), $fileName)
    }

    & (Join-Path $PSScriptRoot 'package-osx-x64-app.ps1') `
        -SourceDirectory $sourceDirectory `
        -OutputDirectory $outputDirectory `
        -BundleVersion '1.28.82.64648' `
        -ShortVersion '1.28.82' `
        -ArchiveTimestamp ([DateTimeOffset]'2026-09-15T07:03:17Z') | Out-Null

    $plistPath = Join-Path $outputDirectory 'IronyModManager.app\Contents\Info.plist'
    $mainExecutable = Join-Path $outputDirectory 'IronyModManager.app\Contents\MacOS\IronyModManager'
    $launcherPath = Join-Path $outputDirectory 'IronyModManager.app\Contents\MacOS\IronyModManagerLauncher'
    $archivePath = Join-Path $outputDirectory 'IronyModManager-osx-x64-app.tar.gz'
    if (-not (Test-Path -LiteralPath $plistPath -PathType Leaf) -or
        -not (Test-Path -LiteralPath $mainExecutable -PathType Leaf) -or
        -not (Test-Path -LiteralPath $launcherPath -PathType Leaf)) {
        throw 'The generated app bundle is incomplete.'
    }

    [xml]$plist = Get-Content -LiteralPath $plistPath -Raw
    $keys = @($plist.plist.dict.key)
    $values = @($plist.plist.dict.string)
    if ($values[$keys.IndexOf('CFBundleExecutable')] -ne 'IronyModManagerLauncher' -or
        $values[$keys.IndexOf('CFBundleShortVersionString')] -ne '1.28.82' -or
        $values[$keys.IndexOf('CFBundleVersion')] -ne '1.28.82.64648') {
        throw 'Info.plist did not contain the expected executable and version metadata.'
    }

    $launcherBytes = [IO.File]::ReadAllBytes($launcherPath)
    $launcher = [Text.Encoding]::UTF8.GetString($launcherBytes)
    if (($launcherBytes[0..2] -join ',') -eq '239,187,191' -or $launcher.Contains("`r")) {
        throw 'The launcher must be UTF-8 without a BOM and use Unix line endings.'
    }
    if (-not $launcher.StartsWith("#!/bin/sh`n", [StringComparison]::Ordinal)) {
        throw 'The launcher must have a macOS-compatible shebang.'
    }
    foreach ($expectedLine in @(
        'launcher_directory=${launcher_path%/*}',
        'launcher_directory=$(CDPATH= cd "$launcher_directory" 2>/dev/null && pwd -P) || exit 1',
        'cd "$launcher_directory" || exit 1',
        'exec "$launcher_directory/IronyModManager" "$@"')) {
        if (-not $launcher.Contains($expectedLine)) {
            throw "The launcher is missing required path/argument behavior: $expectedLine"
        }
    }

    if ((Get-FileHash -LiteralPath $mainExecutable -Algorithm SHA256).Hash -ne
        (Get-FileHash -LiteralPath (Join-Path $sourceDirectory 'IronyModManager') -Algorithm SHA256).Hash) {
        throw 'The real Irony executable changed while assembling the app bundle.'
    }

    $modes = @{}
    $archiveStream = [IO.File]::OpenRead($archivePath)
    try {
        $gzipStream = [IO.Compression.GZipStream]::new($archiveStream, [IO.Compression.CompressionMode]::Decompress)
        try {
            $reader = [System.Formats.Tar.TarReader]::new($gzipStream)
            try {
                while ($entry = $reader.GetNextEntry()) {
                    $modes[$entry.Name] = [int]$entry.Mode
                }
            }
            finally {
                $reader.Dispose()
            }
        }
        finally {
            $gzipStream.Dispose()
        }
    }
    finally {
        $archiveStream.Dispose()
    }

    foreach ($path in @(
        'IronyModManager.app/Contents/MacOS/IronyModManagerLauncher',
        'IronyModManager.app/Contents/MacOS/IronyModManager',
        'IronyModManager.app/Contents/MacOS/IronyModManager.Updater',
        'IronyModManager.app/Contents/MacOS/IronyModManager.GameHandler',
        'IronyModManager.app/Contents/MacOS/libhostfxr.dylib')) {
        if (($modes[$path] -band 73) -ne 73) {
            throw "Executable mode bits were not preserved for $path."
        }
    }

    if (($modes['IronyModManager.app/Contents/MacOS/IronyModManager.dll'] -band 73) -ne 0) {
        throw 'A managed payload file was incorrectly marked executable.'
    }

    $firstArchiveHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash
    & (Join-Path $PSScriptRoot 'package-osx-x64-app.ps1') `
        -SourceDirectory $sourceDirectory `
        -OutputDirectory $outputDirectory `
        -BundleVersion '1.28.82.64648' `
        -ShortVersion '1.28.82' `
        -ArchiveTimestamp ([DateTimeOffset]'2026-09-15T07:03:17Z') | Out-Null
    if ((Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash -ne $firstArchiveHash) {
        throw 'Repeated packaging with identical inputs did not produce an identical archive.'
    }

    Write-Output 'macOS app packaging test passed.'
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}
