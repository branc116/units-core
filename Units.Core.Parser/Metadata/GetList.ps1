function Start-UnitsNetIntegration {
    $old = Get-Location;
    Set-Location $PSScriptRoot;
    if (-not (Test-Path .bin)) {
        mkdir .bin;
    }
    Set-Location .bin;
    if (-not (Test-Path UnitsNet)) {
        git clone https://github.com/angularsen/UnitsNet.git
    }else {
        Set-Location UnitsNet;
        git pull;
        Set-Location ..;
    }
    $units = Get-ChildItem UnitsNet/Common/UnitDefinitions | Where-Object name -like *.json;
    $list = "public static List<string> UnitsNetUnits = new List<string> {" +
        ($units | ForEach-Object -Begin {$l = ""} -Process {$l = $l + '"' + $_.Name.Replace(".json", "") + '",' } -End {return $l.TrimEnd(",")} ) +
        "};";

    $csfile = @"
using System.Collections.Generic;
namespace Units.Core.Parser.Metadata {
    /// <summary>
    /// Data taken from UnitsNet team, Ty UnitsNet team <see cref="https://github.com/angularsen/UnitsNet"/> :) 
    /// </summary>
    public static class UnitsNetAllUnits {
        /// <summary>
        /// List of units defined by UnitsNet team
        /// </summary>
        $list
    }
}
"@;
    Set-Location ..
    $csfile > "UnitsNetAllunits.g.cs";
    Set-Location $old;
}
$done = Measure-Command { Start-UnitsNetIntegration };
Write-Output "Integrated with UnitsNet in $($done.TotalMilliseconds)ms";
