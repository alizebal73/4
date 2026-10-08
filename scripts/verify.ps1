$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,
        [Parameter(Mandatory = $false)]
        [string[]]$ArgumentList = @()
    )

    & $FilePath @ArgumentList

    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code $LASTEXITCODE : $FilePath $($ArgumentList -join ' ')"
    }
}

Invoke-Checked -FilePath "dotnet" -ArgumentList @("--version")
Invoke-Checked -FilePath "dotnet" -ArgumentList @("restore", "GameNet.slnx")
Invoke-Checked -FilePath "dotnet" -ArgumentList @("build", "GameNet.slnx", "--configuration", "Release", "--no-restore")
Invoke-Checked -FilePath "dotnet" -ArgumentList @("test", "GameNet.slnx", "--configuration", "Release", "--no-build", "--no-restore")

Write-Host "LOCAL FOUNDATION BUILD/TEST GATE PASSED."
Write-Host "Runtime certification remains a separate Windows evidence gate."
