# build-and-coverage.ps1

# Exécuter le build et les tests
dotnet build
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Définir le chemin vers ReportGenerator 5.4.3 pour .NET 8
$reportGeneratorPath = Join-Path $env:USERPROFILE ".nuget\packages\reportgenerator\5.4.3\tools\net8.0\reportgenerator.exe"

# Exécuter ReportGenerator
& $reportGeneratorPath `
    -reports:./TestResults/**/coverage.cobertura.xml `
    -targetdir:./CoverageReport `
    -reporttypes:"Html;Cobertura"

Write-Host "`nRapport de couverture généré dans ./CoverageReport" -ForegroundColor Green