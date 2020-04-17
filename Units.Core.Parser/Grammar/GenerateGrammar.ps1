$oldLoc = Get-Location;
Set-Location $PSScriptRoot;
if (Test-Path ".\UnitsGrammar.interp") {
    $ug = Get-Item .\UnitsGrammar.g4;
    $inter = Get-Item .\UnitsGrammar.interp
    if ($ug.LastAccessTime -lt $inter.LastAccessTime) {
        Write-Output "Grammar not changed, will not generate new"
        return;
    }
}
if (-not (Test-Path "./.bin")) {
	mkdir ./.bin
}
if (-not (Test-Path "./.bin/antlr.jar")) {
    Invoke-WebRequest -Uri "https://www.antlr.org/download/antlr-4.8-complete.jar" -OutFile "./.bin/antlr.jar";
}
$fullPath = Resolve-Path "./.bin/antlr.jar";
$env:CLASSPATH = ".:$fullPath";
$a = Measure-Command { java.exe -jar .bin/antlr.jar -Dlanguage=CSharp -visitor .\UnitsGrammar.g4 }
Write-Output "Grammar generated in $($a.Milliseconds)ms";
Set-Location $oldLoc;