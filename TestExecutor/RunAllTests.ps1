#    Delete exiting test results
Get-ChildItem ..\TestResults |
ForEach-Object { 
    if (Test-Path -Path $_.FullName -PathType Container) 
    {
        [IO.Directory]::Delete($_.FullName, $true)
    }
    else 
    {
        Remove-Item $_.FullName -Force
    }
}

#    Delete exiting logs
Get-ChildItem ..\DataManagerAPI.Tests\bin\Debug\net7.0 | 
Where-Object -FilterScript {$_.Name -LIKE "*.log"} |
ForEach-Object { 
    Remove-Item $_.FullName
}

#   run solution build
dotnet build ..\DataManagerAPI.sln --configuration Debug

#   list of directories which contain configuration files ("appsettings.Test.json" and "NLog.Test.json")
$directories = 
    '..\DataManagerAPI',
    '..\DataManagerAPI.gRPCClient\bin\Debug\net7.0',
    '..\DataManagerAPI.gRPCServer\bin\Debug\net7.0',
    '..\DataManagerAPI.NLogger\bin\Debug\net7.0',
    '..\DataManagerAPI.PostgresDB\bin\Debug\net7.0',
    '..\DataManagerAPI.Repository\bin\Debug\net7.0',
    '..\DataManagerAPI.SQLServerDB\bin\Debug\net7.0',
    '..\DataManagerAPI.Tests\bin\Debug\net7.0',
    '..\DataManagerAPI\bin\Debug\net7.0'

#   enumerate all possible configurations
for ($i = 1; $i -le 8; $i++)
{
    # copy "appsettings.Test.json" to apropriate directories
    Write-Host `r`n
    Get-ChildItem .\Configurations | 
    Where-Object -FilterScript {$_.Name -LIKE "$i-Apps-*"} |
    ForEach-Object { 
        Write-Host $_.FullName
        foreach ($dir in $directories) {
            Copy-Item -Path $_.FullName -Destination  $dir\"appsettings.Test.json"
        }
    }
    
    # copy "NLog.Test.json" to apropriate directories
    Get-ChildItem .\Configurations | 
    Where-Object -FilterScript {$_.Name -LIKE "$i-NLog-*"} |
    ForEach-Object { 
        Write-Host $_.FullName
        foreach ($dir in $directories) {
            Copy-Item -Path $_.FullName -Destination  $dir\"NLog.Test.json"
        }
    }

    # run tests for current configuration
    .\RunOneTest.ps1
}

#   move all *.coverage files to TestResults folder
Get-ChildItem ..\TestResults -Recurse -File |
Where-Object -FilterScript { $_.Name -Like "*.coverage"} |
ForEach-Object { 
    Move-Item $_.FullName -Destination ..\TestResults
}

# remove subfolders from TestResults folder
Get-ChildItem ..\TestResults -Recurse -Directory |
ForEach-Object { 
    Remove-Item $_.FullName
}

Write-Host "Finished"