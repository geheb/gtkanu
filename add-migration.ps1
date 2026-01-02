Param(
    [Parameter(Mandatory=$True)]
    [string]$Name
)
Push-Location
cd src/GtKanu.Core
dotnet ef migrations add $Name --startup-project ../GtKanu.WebApp/
Pop-Location