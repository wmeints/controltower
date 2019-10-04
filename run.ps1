# --------------------------------------------------------------------------------------------------------------------
# <copyright file="build.ps1" company="Tygertec">
#   Copyright © 2016 Ty Walls.
#   All rights reserved.
# </copyright>
# <summary>
#   Script for kicking off the command line build.
# </summary>
# --------------------------------------------------------------------------------------------------------------------

param([Parameter(Position = 0, Mandatory = $false)] [string[]]$taskList = @())

$nuget = "./.nuget/nuget.exe"

if (!(test-path "./packages")) {
    mkdir "./packages"
}

if (!(test-path $nuget)) {
    Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile $nuget
}

& $nuget install psake -OutputDirectory ./packages -verbosity quiet

Remove-Module [p]sake

# Find path to psake
$psakeModule = (Get-ChildItem ("./packages/psake*/tools/psake/psake.psm1")).FullName | Sort-Object $_ | Select-Object -Last 1

Import-Module $psakeModule

Invoke-psake `
    -buildFile ./default.ps1 `
    -taskList $taskList `
    -properties @{
    "buildConfiguration" = "Release"
    "buildPlatform"      = "Any CPU"
}

Write-Output ("`r`nBuild finished with code: {0}" -f $LASTEXITCODE)
exit $LASTEXITCODE