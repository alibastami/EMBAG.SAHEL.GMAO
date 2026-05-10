# Build Script for Standalone, Obfuscated Sahel GMAO
# This script publishes the web project as a self-contained folder and applies obfuscation.

# 1. Setup absolute paths
$ProjectRoot = Get-Item .
$ProjectName = "Sahel.GMAO.Web"
$ProjectPath = Join-Path $ProjectRoot "Sahel.GMAO.Web\Sahel.GMAO.Web.csproj"
$OutDir = Join-Path $ProjectRoot "Dist"
$BuildDir = Join-Path $ProjectRoot "Sahel.GMAO.Web\bin\Release\net8.0\win-x64"
$PublishDir = Join-Path $BuildDir "publish"
$ObfuscarExe = "obfuscar.console"

# Cleanup
Write-Host "Cleaning up Dist and Build folders..." -ForegroundColor Cyan
if (Test-Path $OutDir) { Remove-Item -Recurse -Force $OutDir }
if (Test-Path $PublishDir) { Remove-Item -Recurse -Force $PublishDir }
New-Item -ItemType Directory -Path $OutDir | Out-Null

# 2. Restore and Build with specific RID to ensure consistency
Write-Host "Restoring and Building (Self-Contained) for win-x64..." -ForegroundColor Cyan
dotnet restore $ProjectPath -r win-x64
if ($LASTEXITCODE -ne 0) { Write-Error "Restore failed!"; exit $LASTEXITCODE }

dotnet build $ProjectPath -c Release -r win-x64 --no-restore --self-contained true
if ($LASTEXITCODE -ne 0) { Write-Error "Build failed!"; exit $LASTEXITCODE }

# 3. Obfuscate
Write-Host "Applying obfuscation..." -ForegroundColor Cyan
if (-not (Get-Command $ObfuscarExe -ErrorAction SilentlyContinue)) {
    Write-Error "Obfuscar not found. Please install it via NuGet (dotnet tool install --global Obfuscar.GlobalTool)."
    exit 1
}

# Run Obfuscar from the project directory
Push-Location "$ProjectRoot\Sahel.GMAO.Web"
& $ObfuscarExe obfuscar.xml
Pop-Location

if ($LASTEXITCODE -ne 0) {
    Write-Error "Obfuscation failed!"
    exit $LASTEXITCODE
}

# 4. Replace original DLLs with obfuscated ones
Write-Host "Replacing assemblies with obfuscated versions..." -ForegroundColor Cyan
if (Test-Path "$BuildDir\Obfuscated") {
    Copy-Item "$BuildDir\Obfuscated\Sahel.GMAO.*.dll" "$BuildDir\" -Force
} else {
    Write-Error "Obfuscated directory not found at $BuildDir\Obfuscated."
    exit 1
}

# 5. Publish (Self-contained standalone folder with obfuscated DLLs)
Write-Host "Publishing as self-contained standalone folder..." -ForegroundColor Cyan
dotnet publish $ProjectPath -c Release -r win-x64 --no-build --self-contained true /p:PublishSingleFile=false /p:PublishReadyToRun=false /p:PublishTrimmed=false

if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed!"; exit $LASTEXITCODE }

# 6. Finalize
$FinalDir = Join-Path $OutDir "Sahel_GMAO_Portable"
if (Test-Path $PublishDir) {
    Write-Host "Packaging folder to $FinalDir..." -ForegroundColor Cyan
    Copy-Item -Path $PublishDir -Destination $FinalDir -Recurse -Force

    # Ensure Logs folder exists
    New-Item -ItemType Directory -Path (Join-Path $FinalDir "Logs") -Force | Out-Null
    
    # Copy Assets
    Write-Host "Copying assets (listmachine.txt, Word templates)..." -ForegroundColor Cyan
    Copy-Item -Path (Join-Path $ProjectRoot "listmachine.txt") -Destination $FinalDir -Force
    if (Test-Path (Join-Path $ProjectRoot "SAHEL_GMAO_WORD")) {
        Copy-Item -Path (Join-Path $ProjectRoot "SAHEL_GMAO_WORD") -Destination $FinalDir -Recurse -Force
    }
    
    Write-Host "`n====================================================" -ForegroundColor White
    Write-Host "SUCCESS: Standalone GMAO app created!" -ForegroundColor Green
    Write-Host "Location: $FinalDir" -ForegroundColor White
    Write-Host "To run: Launch Sahel.GMAO.exe in that folder." -ForegroundColor White
    Write-Host "====================================================`n" -ForegroundColor White
}
 else {
    Write-Error "Publish directory not found at $PublishDir"
    exit 1
}
