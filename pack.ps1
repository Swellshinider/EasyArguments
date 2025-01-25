# Navigate to the project directory
Set-Location -Path ".\EasyArguments"

# Pack the project in Release mode
dotnet pack -c Release

$packagePath = Get-ChildItem -Filter *.nupkg -Recurse | Sort-Object LastWriteTime -Descending | Select-Object -First 1

# Check if the package was found
if (-not $packagePath) {
    Write-Host "Package not found. Please check if the package was created."
    Set-Location -Path "..\"
    exit 1
}

# Get the nuget key from the environment variable
$nugetApiKey = $env:EasyArguments_nugetkey

# Check if the nuget key is set
if (-not $nugetApiKey) {
    Write-Host "Nuget key not set. Please set the environment variable 'EasyArguments_nugetkey' with the nuget key."
    Set-Location -Path "..\"
    exit 1
}

# Ask for confirmation to publish the package
$confirmation = Read-Host "Do you want to publish the package? [Y/n]"

if ($confirmation -ne 'Y' -and $confirmation -ne 'y' -and $confirmation -ne '') {
    Write-Host "Publishing cancelled."
    Set-Location -Path "..\"
    exit 0
}

# Publish the package
dotnet nuget push $packagePath --source "https://api.nuget.org/v3/index.json" --api-key $nugetApiKey --skip-duplicate

Set-Location -Path "..\"