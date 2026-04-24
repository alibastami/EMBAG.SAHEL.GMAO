$appName = "Sahel.GMAO.Web"

Write-Host "--- Sahel GMAO: Clean Startup ---" -ForegroundColor Cyan

# 1. Kill any existing instance of the app itself (Targeted and Safe)
$procs = Get-Process $appName -ErrorAction SilentlyContinue
if ($procs) {
    Write-Host "Found $($procs.Count) instance(s) of $appName. Closing them gracefully..." -ForegroundColor Yellow
    $procs | Stop-Process -Force
}

# 2. Build the project
Write-Host "--- Building ---" -ForegroundColor Cyan
dotnet build -v q
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# 3. Start (Run EXE directly)
Write-Host "--- Launching Application ---" -ForegroundColor Green
$exePath = ".\bin\Debug\net8.0\Sahel.GMAO.Web.exe"
if (Test-Path $exePath) {
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    # App has auto-browser launch and port fallback built-in
    & $exePath
} else {
    Write-Host "Executable not found at $exePath. Falling back to dotnet run..." -ForegroundColor Yellow
    dotnet run --no-build --launch-profile http
}
