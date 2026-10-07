param(
    [string]$ServerUrl = "http://127.0.0.1:5080"
)

$ErrorActionPreference = "Stop"

Write-Host "== GameNet 4 foundation verification =="

dotnet --version
dotnet restore GameNet.slnx
dotnet build GameNet.slnx --configuration Release --no-restore
dotnet test tests/Domain.Tests/GameNet.Domain.Tests.csproj --configuration Release --no-build

$health = Invoke-RestMethod -Uri "$ServerUrl/api/health" -Method Get
$health | ConvertTo-Json

if ($health.status -ne "ok") {
    throw "Server health is not ok."
}

Write-Host "Foundation verification passed."
