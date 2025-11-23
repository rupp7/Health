<#
Create a zip suitable for submission. The script copies the required source files to a temporary staging folder (excluding bin/obj) and compresses them.
Run from the repository root.
#>
param(
    [string]$OutputZip = "Health_Submission.zip"
)

$root = (Get-Location).Path
$staging = Join-Path $env:TEMP ("HealthSubmission_" + [guid]::NewGuid().ToString())
New-Item -ItemType Directory -Force -Path $staging | Out-Null

# Projects/folders to include
$include = @('Health.sln','HealthMaui','Health.Api','Health.Core','CLI.Health','README.md','docs')

Write-Host "Staging files to: $staging"

foreach ($item in $include) {
    $src = Join-Path $root $item
    if (-not (Test-Path $src)) { Write-Host "Skipping missing path: $item"; continue }

    if ((Get-Item $src).PSIsContainer) {
        Write-Host "Copying folder: $item"
        # copy all files recursively except bin/ and obj/
        Get-ChildItem -Path $src -Recurse -File | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\' } | ForEach-Object {
            $rel = $_.FullName.Substring($root.Length).TrimStart('\')
            $dest = Join-Path $staging $rel
            $destDir = Split-Path $dest -Parent
            if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Force -Path $destDir | Out-Null }
            Copy-Item -Path $_.FullName -Destination $dest -Force
        }
    }
    else {
        Write-Host "Copying file: $item"
        Copy-Item -Path $src -Destination (Join-Path $staging $item) -Force
    }
}

Write-Host "Creating zip: $OutputZip"
if (Test-Path $OutputZip) { Remove-Item $OutputZip -Force }
Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $OutputZip -Force
Write-Host "Created $OutputZip"

# cleanup staging
Remove-Item -Recurse -Force $staging
Write-Host "Staging folder removed"
