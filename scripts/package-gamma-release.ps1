# Packages Dizzy.Gamma for GitHub Releases.
# Usage: .\scripts\package-gamma-release.ps1 [-Version 0.2.0]

param(
    [string]$Version = "0.2.0"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
Set-Location $root

$dll = "src\Dizzy.Gamma\bin\Release\Dizzy.Gamma.dll"
if (-not (Test-Path $dll)) {
    Write-Host "Building Dizzy.Gamma Release..."
    dotnet build src\Dizzy.Gamma\Dizzy.Gamma.csproj -c Release
}

if (-not (Test-Path $dll)) {
    throw "Missing $dll - build failed."
}

$staging = "dist\Dizzy.Gamma-$Version"
$pluginDir = "$staging\Dizzy.Gamma"
$zipPath = "dist\Dizzy.Gamma-$Version.zip"
$notesPath = "dist\GITHUB_RELEASE_NOTES-v$Version.md"

if (Test-Path $staging) {
    Remove-Item $staging -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
Copy-Item $dll $pluginDir -Force

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}
Compress-Archive -Path $pluginDir -DestinationPath $zipPath -Force

$notes = @"
## Install

Requires Sailwind + [BepInEx 5](https://thunderstore.io/c/sailwind/p/BepInEx/BepInExPack/) (Thunderstore BepInExPack recommended).

1. Download `Dizzy.Gamma-$Version.zip` below.
2. Extract the `Dizzy.Gamma` folder into `BepInEx\plugins\` (next to `Sailwind.exe`: `Sailwind\BepInEx\plugins\Dizzy.Gamma\`).
3. Launch the game.

## Contents

```
Dizzy.Gamma/
  Dizzy.Gamma.dll
```

## In-game

| Action | Default |
|--------|---------|
| Settings panel | `F7` |
| Gamma up | `Right Ctrl` + `=` |
| Gamma down | `Right Ctrl` + `-` |

`1.0` = vanilla brightness. Config persists in `BepInEx\config\com.dizzy.sailwind.gamma.cfg`.

## Source

Built from tag `v$Version` on this repository.
"@

New-Item -ItemType Directory -Force -Path dist | Out-Null
Set-Content -Path $notesPath -Value $notes -Encoding UTF8

Write-Host "Created: $zipPath"
Write-Host "Release notes: $notesPath"
Write-Host ""
Write-Host "Next: GitHub -> Releases -> Draft new release -> tag v$Version -> attach zip -> paste notes."
