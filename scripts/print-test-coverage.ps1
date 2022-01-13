$reports = Get-ChildItem -Path $args[0] -Recurse -Filter "coverage.cobertura.xml"
$reports | ForEach-Object {
    $coverage = Select-XML -Path $_ -XPath '/coverage' | ForEach-Object {
        Write-Host "COVERAGE: $(($_.Node.'line-rate' -as [double]) * 100)%"
    }
}