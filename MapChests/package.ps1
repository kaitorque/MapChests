# Builds the plugin, copies it into the VP r2modman profile's Kaitorque-MapChests folder and zips an r2modman-importable package into the parent folder next to the other mods' zips.
$ErrorActionPreference = "Stop"
$name = "MapChests"
$dir = $PSScriptRoot
$root = Split-Path $dir -Parent
$profilePlugins = Join-Path $env:APPDATA "r2modmanPlus-local\RiskOfRain2\profiles\VP\BepInEx\plugins"

dotnet build (Join-Path $dir "$name.csproj") -c Release --nologo -v q
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

$dll = Join-Path $dir "bin\Release\netstandard2.1\$name.dll"

# Same folder r2modman uses when the zip is imported, so only one copy is loaded and r2modman can toggle it
$install = Join-Path $profilePlugins "Kaitorque-$name"
New-Item -ItemType Directory -Force $install | Out-Null
$stray = Join-Path $profilePlugins $name
if (Test-Path $stray) { Remove-Item $stray -Recurse -Force }
Copy-Item $dll $install -Force
Write-Host "Installed $install\$name.dll"

$package = Join-Path $dir "package"
if (Test-Path $package) { Remove-Item $package -Recurse -Force }
New-Item -ItemType Directory $package | Out-Null
Copy-Item $dll, (Join-Path $dir "manifest.json"), (Join-Path $root "icon.png"), (Join-Path $root "README.md") $package

$version = (Get-Content (Join-Path $dir "manifest.json") -Raw | ConvertFrom-Json).version_number
$zip = Join-Path (Split-Path $root -Parent) "Kaitorque-$name-$version.zip"
if (Test-Path $zip) { Remove-Item $zip }
Compress-Archive -Path (Join-Path $package "*") -DestinationPath $zip
Write-Host "Packaged $zip"
