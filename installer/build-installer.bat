@echo off
setlocal enabledelayedexpansion
title Build WIDM TestApp installer

rem Run from anywhere - hop to the repo root (this script lives in installer\).
cd /d "%~dp0.."

echo ============================================================
echo   Building WIDM Test Applicatie installer
echo ============================================================
echo.

echo [1/2] Publishing self-contained win-x64 build...
dotnet publish "TestApp\TestApp.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false
if errorlevel 1 (
  echo.
  echo ERROR: dotnet publish failed. Is the .NET 10 SDK installed?
  exit /b 1
)

echo.
echo [2/2] Compiling Inno Setup installer...
set "ISCC=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
if not exist "!ISCC!" set "ISCC=%ProgramFiles%\Inno Setup 6\ISCC.exe"
if not exist "!ISCC!" (
  echo.
  echo ERROR: Inno Setup 6 ^(ISCC.exe^) was not found in Program Files.
  echo Install it from https://jrsoftware.org/isdl.php and run this script again.
  exit /b 1
)

"!ISCC!" "installer\TestApp.iss"
if errorlevel 1 (
  echo.
  echo ERROR: Inno Setup compilation failed.
  exit /b 1
)

echo.
echo ============================================================
echo   Done. Find the installer in: installer\Output\
echo ============================================================
endlocal
