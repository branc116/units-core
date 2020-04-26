$old = Get-Location
Set-Location $PSScriptRoot
mkdir .bin;
cd .bin;

Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/v5.5.1/nuget.exe" -OutFile nuget.exe
$nuget = Resolve-Path nuget.exe;

Set-Location ..\..\Units.Core
rm ./bin -force -recurse
dotnet pack


Set-Location $old