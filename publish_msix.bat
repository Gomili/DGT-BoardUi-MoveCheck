@echo off
echo ============================================================
echo MSIX-Bundle-Paketierung fuer SchachZugCheckerWinUI
echo ============================================================
echo.
echo Dieser Prozess erstellt ein MSIX-Bundle (.msixbundle)
echo welches sowohl x64 als auch ARM64 Architekturen enthaelt.
echo.
echo HINWEIS: Ein Test-Zertifikat wurde erstellt und 
echo in die Projektdatei eingebunden.
echo.
echo.
echo HINWEIS: Dieses Skript nutzt das neue Packaging-Projekt (.wapproj).
echo.
echo Starte Build-Prozess (erfordert MSBuild/Visual Studio Build Tools)...
echo.

:: Wir nutzen MSBuild fuer .wapproj, da dotnet CLI diese oft nicht nativ unterstuetzt.
:: Falls MSBuild nicht im Pfad ist, wird versucht dotnet zu nutzen.
MSBuild SchachZugCheckerWinUI.Package\SchachZugCheckerWinUI.Package.wapproj /p:Configuration=Release /p:AppxBundle=Always /p:AppxBundlePlatforms="x64|arm64" /p:GenerateAppxPackageOnBuild=true

if %errorlevel% neq 0 (
    echo MSBuild fehlgeschlagen, versuche dotnet build...
    dotnet build SchachZugCheckerWinUI.Package\SchachZugCheckerWinUI.Package.wapproj -c Release -p:AppxBundle=Always -p:AppxBundlePlatforms="x64|arm64"
)

echo.
echo ============================================================
echo Fertig! Das Bundle (.msixbundle) findest du hier:
echo SchachZugCheckerWinUI.Package\AppPackages\
echo ============================================================
pause