; Inno Setup script for the WIDM Test Applicatie (.NET 6 WinForms client).
;
; Packages a self-contained publish (the .NET runtime is bundled, so target
; machines need NOTHING pre-installed) into a single setup.exe with Start Menu
; and optional desktop shortcuts, plus an uninstaller.
;
; Build it with installer\build-installer.bat, or compile this file directly in
; the Inno Setup Compiler after running the publish step (see installer\README.md).
;
; Edit the five values below to change branding/version.

#define MyAppName "WIDM Test Applicatie"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "F. van den Berg"
#define MyAppExeName "TestApp.exe"
; Publish output, relative to this .iss file. Produced by:
;   dotnet publish TestApp\TestApp.csproj -c Release -r win-x64 --self-contained true
#define MyPublishDir "..\TestApp\bin\Release\net10.0-windows\win-x64\publish"

[Setup]
; AppId uniquely identifies this application for upgrades/uninstall. NEVER change
; it once released, or Windows will treat new versions as a separate product.
AppId={{f2a9d4e7-6c31-4b8e-9a5f-1e0c7d3b28a4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\WIDM TestApp
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
UninstallDisplayName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
OutputDir=Output
OutputBaseFilename=WIDM-TestApp-Setup-{#MyAppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Self-contained build is 64-bit only.
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
; Machine-wide install to Program Files needs elevation (prompts UAC once).
PrivilegesRequired=admin
; Offer to close a running instance so files can be replaced during upgrades.
CloseApplications=yes

[Languages]
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Everything the publish step produced (app exe, bundled runtime, EF Core SQLite
; native library, etc.). ignoreversion keeps our files during upgrades.
Source: "{#MyPublishDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

; NOTE: user data lives in %USERPROFILE%\Documents\widmTest (app.db and backups),
; outside the install directory, so uninstalling deliberately leaves it untouched.
