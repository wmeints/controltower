Properties {
    $Configuration = "Release"
    $BuildFolder = Split-Path $psake.build_script_file
    $SourceFolder = "$BuildFolder/src"
    $TestsFolder = "$BuildFolder/tests"
    $SolutionFile = "$BuildFolder/ControlTower.sln"
}

Task RestorePackages {
    Exec {
        dotnet restore $SolutionFile
    }
}

Task Compile -Depends RestorePackages {
    Exec {
        dotnet build $SolutionFile -c $Configuration --no-restore
    }
}

Task Tests -Depends Compile {
    $TestProjects = Get-ChildItem "$TestsFolder/*.Tests/*.csproj"

    foreach ($TestProject in $TestProjects) {
        Exec {
            dotnet test $TestProject.FullName -c Release --no-restore
        }
    }
}

Task default -Depends Tests