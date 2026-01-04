Param(
    [Parameter(Mandatory=$True)]
    [string]$Name
)
Push-Location
cd src/GtKanu.Infrastructure
dotnet ef migrations add $Name -o Database/Migrations --startup-project ../GtKanu.WebApp/
Pop-Location