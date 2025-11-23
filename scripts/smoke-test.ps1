<#
Simple smoke test for Health.Api. Assumes the API is already running at http://localhost:7009.
This script POSTs a test patient, fetches the list, checks that the patient is returned, and then deletes the patient.
#>
param(
    [string]$ApiBase = 'http://localhost:7009'
)

Write-Host "Running smoke test against $ApiBase"

$sample = @{ id = ([guid]::Empty); firstName='Smoke'; lastName='Test'; birthDate='1990-01-01'; address='1 Demo St'; race='Unknown'; gender='Unknown'; notes=@() }
$body = $sample | ConvertTo-Json -Depth 10

try {
    $created = Invoke-RestMethod -Uri "$ApiBase/api/patients" -Method Post -Body $body -ContentType 'application/json'
    Write-Host "Created patient id: $($created.id)"

    $all = Invoke-RestMethod -Uri "$ApiBase/api/patients" -Method Get
    $count = if ($all -is [System.Collections.IEnumerable]) { ($all | Measure-Object).Count } else { 0 }
    Write-Host "Patients returned: $count"

    if ($all -and ($all | Where-Object { $_.id -eq $created.id })) {
        Write-Host "Smoke test: PASSED"
    } else {
        Write-Host "Smoke test: FAILED — created record not returned"
    }

    # cleanup
    Invoke-RestMethod -Uri "$ApiBase/api/patients/$($created.id)" -Method Delete
    Write-Host "Deleted sample patient"
}
catch {
    Write-Host "Error during smoke test: $_"
    exit 1
}
