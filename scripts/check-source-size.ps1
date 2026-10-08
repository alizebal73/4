$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$maxLines = 500
$maxBytes = 40KB
$extensions = @(".cs", ".xaml", ".tsx", ".ts", ".jsx", ".js", ".css")
$root = (Resolve-Path "$PSScriptRoot\..").Path
$excludedSegments = @("\bin\", "\obj\", "\TestResults\", "\Migrations\")
$violations = @()

Get-ChildItem -Path $root -Recurse -File |
    Where-Object { $extensions -contains $_.Extension.ToLowerInvariant() } |
    ForEach-Object {
        $fullPath = $_.FullName

        if ($excludedSegments | Where-Object { $fullPath.Contains($_) }) {
            return
        }

        $lineCount = (Get-Content -LiteralPath $fullPath | Measure-Object -Line).Lines
        $byteCount = $_.Length

        if ($lineCount -gt $maxLines -or $byteCount -gt $maxBytes) {
            $relative = [System.IO.Path]::GetRelativePath($root, $fullPath)
            $violations += "$relative => $lineCount lines, $byteCount bytes"
        }
    }

if ($violations.Count -gt 0) {
    throw ("Source-size guard failed. Split oversized hand-authored source files:" + [Environment]::NewLine + ($violations -join [Environment]::NewLine))
}

Write-Host "SOURCE SIZE GATE PASSED: no hand-authored source file exceeds $maxLines lines or $maxBytes bytes."
