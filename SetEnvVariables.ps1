if (!(Test-Path -Path "secrets.json")) {
    Write-Host "Secrets file not found"
    Exit
}
$secrets = Get-Content -Raw secrets.json | ConvertFrom-Json -AsHashtable
foreach ($kv in $secrets.GetEnumerator() )
{
    Write-Host "Setting: $($kv.Name)"
    [Environment]::SetEnvironmentVariable($($kv.Name), $($kv.Value), "Machine")
}