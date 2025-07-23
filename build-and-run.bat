@echo off
echo Building Stylish Calculator...
dotnet build StylishCalculator.csproj -c Release

if %ERRORLEVEL% NEQ 0 (
    echo Build failed with error code %ERRORLEVEL%
    pause
    exit /b %ERRORLEVEL%
)

echo Build successful!
echo.
echo Running Stylish Calculator...
echo.
start "" "bin\Release\net6.0-windows\StylishCalculator.exe"

echo.
echo If the application doesn't start automatically, you can find it at:
echo bin\Release\net6.0-windows\StylishCalculator.exe
echo.
pause