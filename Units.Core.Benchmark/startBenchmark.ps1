param([int]$n, [bool]$SkipBuild)
$old = Get-Location;
try {
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
    dotnet build -c=Release
}
Set-Location ".\bin\Release\netcoreapp3.1";
$date = [system.DateTime]::Now;
$csvName = "benchmark_$($date.year)$($date.month)$($date.day).csv";
$data = "length", "scalar", "float", "lengthD", "scalarD", "double", "unitsnet";
"run, " + ($data -join ", ") > $csvName;
1..$n | % { 
    Write-Progress -Activity "Benchmark" -Status "Running $_ out of $n" -PercentComplete ([int]($_*100/$n));
    $time = $data | % {
        $time1 = (Measure-Command { .\Units.Core.Benchmark.exe $_ }).TotalMilliseconds; 
        return "$time1";
    }
    "$_, " + ($time -join ", ") >> $csvName;
}
cp $csvName ..\..\..
} finally {
    set-location $old;
}