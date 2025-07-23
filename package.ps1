# PowerShell script to package the Stylish Calculator application
# This creates a self-contained package that can be distributed to users

# Set the output directory
$outputDir = ".\publish"

# Create the output directory if it doesn't exist
if (!(Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
}

Write-Host "Packaging Stylish Calculator for distribution..." -ForegroundColor Cyan

# Build the application as a self-contained package
Write-Host "Building self-contained package..." -ForegroundColor Yellow
dotnet publish StylishCalculator.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishReadyToRun=true `
    -o $outputDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed with error code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Create a ZIP file of the package
Write-Host "Creating ZIP archive..." -ForegroundColor Yellow
$zipFile = ".\StylishCalculator-Release.zip"

if (Test-Path $zipFile) {
    Remove-Item $zipFile -Force
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($outputDir, $zipFile)

Write-Host "Package created successfully!" -ForegroundColor Green
Write-Host "You can find the executable at: $outputDir\StylishCalculator.exe" -ForegroundColor Cyan
Write-Host "ZIP archive created at: $zipFile" -ForegroundColor Cyan

# Open the output directory
explorer $outputDir