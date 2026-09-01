# Palvelulaskuri (SpecialCalculator)

Service cost calculator for **Lapin Koti- ja Mökkiyhdistys ry** (Lapland Home and Cottage Association). Enter work hours and travel kilometers, pick a rate type, and get a VAT-inclusive total.

The same calculator ships as a **.NET MAUI** app (Android, Windows, iOS, Mac Catalyst) and as a **Blazor WebAssembly** PWA under `Web/`.

## Features

- Work cost (hours × hourly rate) and travel cost (km × mileage rate), net of VAT
- Subtotal, VAT (default **25.5%**), and grand total in euro, using Finnish number formatting
- Built-in hourly rates (e.g. perustalkkari, digitalkkari) and mileage rates (e.g. julkinen, perus, peräkärry)
- Admin settings to edit rates and VAT, plus reset to defaults
- Finnish and English UI, light and dark themes
- Settings stored locally (MAUI preferences / browser `localStorage`)

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- For the web client CSS pipeline: [Node.js](https://nodejs.org/) (npm) **or** a `tailwindcss.exe` next to the web project
- For Android: Android workload (`dotnet workload install maui`) and an emulator or device
- Minimum Android API 28

## Installation

Clone the repository and restore workloads:

```bash
git clone <repository-url>
cd SpecialCalculator
dotnet workload restore
```

Open `SpecialCalculator.slnx` in Visual Studio / Cursor, or use the CLI below.

### MAUI app

```bash
# Windows (unpackaged)
dotnet build SpecialCalculator.csproj -f net10.0-windows10.0.19041.0
dotnet run --project SpecialCalculator.csproj -f net10.0-windows10.0.19041.0

# Android
dotnet build SpecialCalculator.csproj -f net10.0-android
dotnet run --project SpecialCalculator.csproj -f net10.0-android
```

### Web (Blazor WASM)

```bash
cd Web/SpecialCalculatorWeb
npm install
dotnet run
```

The Tailwind build runs automatically before `dotnet build` (`npm run build:css`, or `tailwindcss.exe` if present).

## Usage

1. Choose a **work type** and **travel type**.
2. Enter **hours** and **kilometers** (comma or dot as decimal separator). Empty fields count as zero; negatives are rejected.
3. Read the breakdown: work, travel, subtotal (VAT 0%), VAT, and **YHTEENSÄ / TOTAL**.
4. Open **Settings** to change rates and VAT (admin tab) or language and theme (app tab).

Calculation:

```
work    = hours × hourlyRate
travel  = kilometers × mileageRate
subtotal = work + travel
vat     = subtotal × (vatPercent / 100)
total   = subtotal + vat
```

**Clear** resets inputs and restores the default rate selection.

## Android release APK

From the repo root (PowerShell):

```powershell
.\scripts\build-apk.ps1
```

Output: `dist/Palvelulaskuri-<version>-arm64-Signed.apk`.

Optional release signing: copy `SpecialCalculator.AndroidSigning.props.example` to `SpecialCalculator.AndroidSigning.props` and fill in keystore paths and passwords. Do not commit the props file or the keystore.

Install on a connected device:

```bash
adb install -r dist/Palvelulaskuri-1.0-arm64-Signed.apk
```

## Project layout

| Path | Role |
|------|------|
| `SpecialCalculator.csproj` | MAUI client (`Palvelulaskuri`) |
| `Web/SpecialCalculatorWeb/` | Blazor WASM PWA |
| `scripts/build-apk.ps1` | Release APK for `android-arm64` |

## Contributing

Pull requests are welcome. For larger changes, open an issue first.

Please keep MAUI and web behavior in sync (calculation, validation, default rates, localization keys) and update both sides when you change shared rules.

## License

No license file is included in this repository. All rights reserved unless the maintainers add a license.
