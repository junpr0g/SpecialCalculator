# Builds a release APK for modern Android phones (arm64).
# Output: dist/Palvelulaskuri-<version>-arm64-Signed.apk

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "SpecialCalculator.csproj"
$distDir = Join-Path $repoRoot "dist"

Push-Location $repoRoot
try {
    Write-Host "Publishing Android Release APK (arm64)..." -ForegroundColor Cyan

    dotnet publish $project `
        -f net10.0-android `
        -c Release `
        -p:AndroidPackageFormat=apk `
        -p:RuntimeIdentifier=android-arm64

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE"
    }

    $publishDir = Join-Path $repoRoot "bin\Release\net10.0-android\android-arm64\publish"
    $signedApk = Get-ChildItem -Path $publishDir -Filter "*-Signed.apk" | Select-Object -First 1
    if (-not $signedApk) {
        $signedApk = Get-ChildItem -Path $publishDir -Filter "*.apk" | Select-Object -First 1
    }

    if (-not $signedApk) {
        throw "APK not found in $publishDir"
    }

    New-Item -ItemType Directory -Force -Path $distDir | Out-Null

    $csprojText = Get-Content -Path $project -Raw
    $version = "1.0"
    if ($csprojText -match '<ApplicationDisplayVersion>([^<]+)</ApplicationDisplayVersion>') {
        $version = $matches[1].Trim()
    }

    $outputName = "Palvelulaskuri-$version-arm64-Signed.apk"
    $outputPath = Join-Path $distDir $outputName
    Copy-Item -Path $signedApk.FullName -Destination $outputPath -Force

    $sizeMb = [math]::Round((Get-Item $outputPath).Length / 1MB, 2)
    Write-Host ""
    Write-Host "APK ready:" -ForegroundColor Green
    Write-Host "  $outputPath ($sizeMb MB)"
    Write-Host ""
    Write-Host "Install on a connected device:" -ForegroundColor Yellow
    Write-Host "  adb install -r `"$outputPath`""
}
finally {
    Pop-Location
}
