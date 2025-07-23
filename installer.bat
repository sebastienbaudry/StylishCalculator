@echo off
setlocal enabledelayedexpansion

echo.
echo ========================================
echo   Stylish Calculator Installer v1.0
echo ========================================
echo.

:: Check for administrator privileges
net session >nul 2>&1
if %errorLevel% == 0 (
    echo Running with administrator privileges...
) else (
    echo Note: Running without administrator privileges.
    echo Some features may require elevated permissions.
)

:: Set default installation directory
set "INSTALL_DIR=%LOCALAPPDATA%\StylishCalculator"

:: Ask user for installation directory
echo.
echo Default installation directory: %INSTALL_DIR%
set /p "USER_DIR=Enter installation directory (or press Enter for default): "

if not "%USER_DIR%"=="" (
    set "INSTALL_DIR=%USER_DIR%"
)

echo.
echo Installing to: %INSTALL_DIR%
echo.

:: Create installation directory
if not exist "%INSTALL_DIR%" (
    mkdir "%INSTALL_DIR%"
    if errorlevel 1 (
        echo Error: Could not create installation directory.
        pause
        exit /b 1
    )
)

:: Copy files from the release directory
echo Extracting files...
set "SOURCE_DIR=%~dp0bin\Release\net6.0-windows"

if not exist "%SOURCE_DIR%" (
    echo Error: Source files not found at %SOURCE_DIR%
    echo Please ensure the application has been built for release.
    pause
    exit /b 1
)

xcopy /E /I /Y "%SOURCE_DIR%\*" "%INSTALL_DIR%\" >nul
if errorlevel 1 (
    echo Error: Could not copy files to installation directory.
    pause
    exit /b 1
)

:: Create desktop shortcut
echo Creating desktop shortcut...
set "SHORTCUT_PATH=%USERPROFILE%\Desktop\Stylish Calculator.lnk"
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%SHORTCUT_PATH%'); $Shortcut.TargetPath = '%INSTALL_DIR%\StylishCalculator.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.IconLocation = '%INSTALL_DIR%\Resources\calculator.ico'; $Shortcut.Description = 'Stylish Calculator with Currency Converter'; $Shortcut.Save()"

:: Create start menu shortcut
echo Creating start menu shortcut...
set "STARTMENU_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs"
if not exist "%STARTMENU_DIR%" mkdir "%STARTMENU_DIR%"
set "STARTMENU_SHORTCUT=%STARTMENU_DIR%\Stylish Calculator.lnk"
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%STARTMENU_SHORTCUT%'); $Shortcut.TargetPath = '%INSTALL_DIR%\StylishCalculator.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.IconLocation = '%INSTALL_DIR%\Resources\calculator.ico'; $Shortcut.Description = 'Stylish Calculator with Currency Converter'; $Shortcut.Save()"

:: Register uninstaller
echo Registering uninstaller...
set "UNINSTALL_KEY=HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\StylishCalculator"
reg add "%UNINSTALL_KEY%" /v "DisplayName" /t REG_SZ /d "Stylish Calculator" /f >nul
reg add "%UNINSTALL_KEY%" /v "DisplayVersion" /t REG_SZ /d "1.0.0" /f >nul
reg add "%UNINSTALL_KEY%" /v "Publisher" /t REG_SZ /d "Calculator Team" /f >nul
reg add "%UNINSTALL_KEY%" /v "InstallLocation" /t REG_SZ /d "%INSTALL_DIR%" /f >nul
reg add "%UNINSTALL_KEY%" /v "UninstallString" /t REG_SZ /d "\"%INSTALL_DIR%\Uninstall.bat\"" /f >nul
reg add "%UNINSTALL_KEY%" /v "NoModify" /t REG_DWORD /d 1 /f >nul
reg add "%UNINSTALL_KEY%" /v "NoRepair" /t REG_DWORD /d 1 /f >nul

:: Create uninstaller
echo Creating uninstaller...
(
echo @echo off
echo setlocal
echo.
echo echo Uninstalling Stylish Calculator...
echo.
echo :: Remove shortcuts
echo if exist "%USERPROFILE%\Desktop\Stylish Calculator.lnk" del "%USERPROFILE%\Desktop\Stylish Calculator.lnk"
echo if exist "%APPDATA%\Microsoft\Windows\Start Menu\Programs\Stylish Calculator.lnk" del "%APPDATA%\Microsoft\Windows\Start Menu\Programs\Stylish Calculator.lnk"
echo.
echo :: Remove registry entries
echo reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\StylishCalculator" /f ^>nul 2^>^&1
echo.
echo :: Remove installation directory
echo echo Removing installation files...
echo cd /d "%TEMP%"
echo rmdir /s /q "%INSTALL_DIR%"
echo.
echo echo Stylish Calculator has been uninstalled.
echo pause
) > "%INSTALL_DIR%\Uninstall.bat"

echo.
echo ========================================
echo   Installation completed successfully!
echo ========================================
echo.
echo Installation directory: %INSTALL_DIR%
echo Desktop shortcut: Created
echo Start menu shortcut: Created
echo.
echo You can now run Stylish Calculator from:
echo - Desktop shortcut
echo - Start menu
echo - Installation directory: %INSTALL_DIR%\StylishCalculator.exe
echo.
echo To uninstall, use Windows "Add or Remove Programs" or run:
echo %INSTALL_DIR%\Uninstall.bat
echo.
pause