[CmdletBinding()]
param(
    [string]$SourceDirectory,
    [string]$OutputDirectory,
    [string]$BundleVersion,
    [string]$ShortVersion,
    [DateTimeOffset]$ArchiveTimestamp
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))

if ([string]::IsNullOrWhiteSpace($SourceDirectory)) {
    $SourceDirectory = Join-Path $repositoryRoot 'src\IronyModManager\bin\x64\osx-x64\net10.0\publish\osx-x64'
}

$SourceDirectory = [IO.Path]::GetFullPath($SourceDirectory)
if (-not (Test-Path -LiteralPath $SourceDirectory -PathType Container)) {
    throw "The osx-x64 publish payload was not found: $SourceDirectory"
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = [IO.Directory]::GetParent($SourceDirectory).FullName
}

$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null

if ([string]::IsNullOrWhiteSpace($BundleVersion) -or [string]::IsNullOrWhiteSpace($ShortVersion) -or -not $PSBoundParameters.ContainsKey('ArchiveTimestamp')) {
    $versionJson = & nbgv get-version -f json
    if ($LASTEXITCODE -ne 0) {
        throw 'Nerdbank.GitVersioning could not provide bundle version metadata.'
    }

    $version = $versionJson | ConvertFrom-Json
    if ([string]::IsNullOrWhiteSpace($BundleVersion)) {
        $BundleVersion = $version.AssemblyFileVersion
    }

    if ([string]::IsNullOrWhiteSpace($ShortVersion)) {
        $ShortVersion = $version.SimpleVersion
    }

    if (-not $PSBoundParameters.ContainsKey('ArchiveTimestamp')) {
        $ArchiveTimestamp = [DateTimeOffset]$version.GitCommitDate
    }
}

if ($BundleVersion -notmatch '^\d+(\.\d+){1,3}$' -or $ShortVersion -notmatch '^\d+(\.\d+){1,2}$') {
    throw 'The bundle versions must contain only dot-separated numeric components.'
}

$requiredPayload = @(
    'IronyModManager',
    'IronyModManager.Updater',
    'IronyModManager.GameHandler'
)
$launcherName = 'IronyModManagerLauncher'
foreach ($fileName in $requiredPayload) {
    if (-not (Test-Path -LiteralPath (Join-Path $SourceDirectory $fileName) -PathType Leaf)) {
        throw "The required osx-x64 payload file is missing: $fileName"
    }
}

$bundleDirectory = Join-Path $OutputDirectory 'IronyModManager.app'
$contentsDirectory = Join-Path $bundleDirectory 'Contents'
$macOSDirectory = Join-Path $contentsDirectory 'MacOS'
$archivePath = Join-Path $OutputDirectory 'IronyModManager-osx-x64-app.tar.gz'

foreach ($target in @($bundleDirectory, $archivePath)) {
    $fullTarget = [IO.Path]::GetFullPath($target)
    if (-not $fullTarget.StartsWith($OutputDirectory + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to replace a package outside the output directory: $fullTarget"
    }

    if (Test-Path -LiteralPath $fullTarget) {
        Remove-Item -LiteralPath $fullTarget -Recurse -Force
    }
}

[IO.Directory]::CreateDirectory($macOSDirectory) | Out-Null
Get-ChildItem -LiteralPath $SourceDirectory -Force | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $macOSDirectory -Recurse -Force
}

$launcher = @(
    '#!/bin/sh',
    'launcher_path=$0',
    'launcher_directory=${launcher_path%/*}',
    'if [ "$launcher_directory" = "$launcher_path" ]; then launcher_directory=.; fi',
    'launcher_directory=$(CDPATH= cd "$launcher_directory" 2>/dev/null && pwd -P) || exit 1',
    'cd "$launcher_directory" || exit 1',
    'exec "$launcher_directory/IronyModManager" "$@"'
) -join "`n"
[IO.File]::WriteAllText((Join-Path $macOSDirectory $launcherName), $launcher + "`n", [Text.UTF8Encoding]::new($false))

$escapedShortVersion = [Security.SecurityElement]::Escape($ShortVersion)
$escapedBundleVersion = [Security.SecurityElement]::Escape($BundleVersion)
$infoPlist = @"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "https://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDevelopmentRegion</key>
    <string>en</string>
    <key>CFBundleDisplayName</key>
    <string>Irony Mod Manager</string>
    <key>CFBundleExecutable</key>
    <string>$launcherName</string>
    <key>CFBundleIdentifier</key>
    <string>com.bcssov.IronyModManager</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundleName</key>
    <string>IronyModManager</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>$escapedShortVersion</string>
    <key>CFBundleVersion</key>
    <string>$escapedBundleVersion</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
"@
[IO.File]::WriteAllText((Join-Path $contentsDirectory 'Info.plist'), $infoPlist, [Text.UTF8Encoding]::new($false))

$archiveStream = [IO.File]::Create($archivePath)
try {
    $gzipStream = [IO.Compression.GZipStream]::new($archiveStream, [IO.Compression.CompressionLevel]::Optimal, $true)
    try {
        $tarWriter = [System.Formats.Tar.TarWriter]::new($gzipStream, [System.Formats.Tar.TarEntryFormat]::Ustar, $true)
        try {
            $items = @((Get-Item -LiteralPath $bundleDirectory)) + @(Get-ChildItem -LiteralPath $bundleDirectory -Recurse -Force)
            foreach ($item in $items | Sort-Object FullName) {
                $dataStream = $null
                $relativePath = [IO.Path]::GetRelativePath($OutputDirectory, $item.FullName).Replace('\', '/')
                if ($item.PSIsContainer) {
                    $entry = [System.Formats.Tar.UstarTarEntry]::new([System.Formats.Tar.TarEntryType]::Directory, $relativePath + '/')
                    $entry.Mode = [IO.UnixFileMode]493
                }
                else {
                    $entry = [System.Formats.Tar.UstarTarEntry]::new([System.Formats.Tar.TarEntryType]::RegularFile, $relativePath)
                    $leafName = $item.Name
                    $isExecutable = $leafName -eq $launcherName -or $leafName -in $requiredPayload -or $item.Extension -in @('.dylib', '.so')
                    $entry.Mode = [IO.UnixFileMode]($isExecutable ? 493 : 420)
                    $dataStream = [IO.File]::OpenRead($item.FullName)
                    $entry.DataStream = $dataStream
                }

                try {
                    $entry.ModificationTime = $ArchiveTimestamp
                    $tarWriter.WriteEntry($entry)
                }
                finally {
                    if ($null -ne $dataStream) {
                        $dataStream.Dispose()
                    }
                }
            }
        }
        finally {
            $tarWriter.Dispose()
        }
    }
    finally {
        $gzipStream.Dispose()
    }
}
finally {
    $archiveStream.Dispose()
}

Write-Output "macOS app bundle: $bundleDirectory"
Write-Output "Permission-preserving archive: $archivePath"
