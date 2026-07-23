# Installer for the WIDM Test Applicatie

Builds a Windows `setup.exe` for the .NET 6 WinForms client using
[Inno Setup](https://jrsoftware.org/isdl.php).

The published build is **self-contained** — the .NET runtime is bundled, so target
machines need nothing installed beforehand (this matters because .NET 6 is
out of support and won't be present on fresh machines). The trade-off is a larger
installer (~150 MB).

## Prerequisites (build machine only)

- **.NET 10 SDK** to run `dotnet publish` for `net10.0-windows`.
- **Inno Setup 6** — install from https://jrsoftware.org/isdl.php.

## Build

From the repo root (or by double-clicking the script):

```bat
installer\build-installer.bat
```

This runs `dotnet publish` (self-contained, `win-x64`) and then compiles the
installer. The result lands in `installer\Output\WIDM-TestApp-Setup-<version>.exe`.

### Manual steps (equivalent)

```bat
dotnet publish TestApp\TestApp.csproj -c Release -r win-x64 --self-contained true
"%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" installer\TestApp.iss
```

Or open `installer\TestApp.iss` in the Inno Setup Compiler and press **Build**
(after running the publish step at least once).

## What the installer does

- Installs to `C:\Program Files\WIDM TestApp` (prompts for admin once).
- Creates a Start Menu shortcut; a desktop shortcut is offered as an opt-in.
- Registers an entry in *Apps & features* with a working uninstaller.
- Replaces files in place on upgrade (same `AppId`, bump `MyAppVersion`).

User data (`app.db` and backups) lives in `Documents\widmTest`, **outside** the
install folder, so uninstalling leaves it untouched.

## Changing branding / version

Edit the `#define`s at the top of [`TestApp.iss`](TestApp.iss) — app name, version,
publisher, exe name, and the publish path. Do **not** change `AppId` once you've
distributed a build, or Windows will see future versions as a different program.
