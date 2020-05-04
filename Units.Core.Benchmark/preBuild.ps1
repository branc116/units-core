param([int]$n, [bool]$SkipBuild)
$old = Get-Location;
if (-not $n) {
    $n = 10;
}
Set-Location $PSScriptRoot
if (-not $SkipBuild) {
    Set-Location ..
    Set-Location .\Units.Core\
    dotnet build -c=Release
    Set-Location ..\Units.Core.Benchmark
    & "..\Units.Core\bin\Release\netcoreapp3.1\Units.Core.exe" Run
}