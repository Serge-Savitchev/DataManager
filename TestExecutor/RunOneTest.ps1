Get-Location
Push-Location ".."

dotnet test --collect "Code Coverage" --no-build --environment ASPNETCORE_ENVIRONMENT="Test" `
--results-directory .\TestResults --settings CodeCoverage.runsettings `
--configuration Debug

Pop-Location
Get-Location
