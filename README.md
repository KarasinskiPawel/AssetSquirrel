# AssetSquirrel

Wewnętrzna aplikacja do ewidencji i zarządzania sprzętem IT (komputery, monitory, drukarki itp.), pracownikami, przekazaniami sprzętu (handover), fakturami, lokalizacjami, producentami i dostawcami.

## Stos technologiczny

- .NET 8 / C#
- Blazor Server (`AssetSquirrelAuthorize.WebApp`)
- Entity Framework Core + SQL Server (`AssetsSquirrel.Plugins.EFCoreSqlServer`)
- ASP.NET Core Identity (konta użytkowników są zunifikowane z danymi biznesowymi w jednej bazie)
- xUnit + Moq (testy jednostkowe w `AssetSquirrel.UseCases.Tests`)
- Mapster (mapowanie encja ↔ DTO)

## Architektura

Warstwy w stylu Clean Architecture:

```
AssetSquirrel.CoreBusiness   → encje, DTO, Result<T> (brak zależności od EF/DB)
AssetSquirrel.UseCases       → logika biznesowa + interfejsy repozytoriów (PluginInterfaces)
Plugins                      → implementacje repozytoriów (EFCoreSqlServer, InMemory)
AssetSquirrelAuthorize.WebApp → UI Blazor Server + kompozycja DI
```

Szczegółowy opis architektury, konwencji i znanych "dziwactw" repo — patrz `CLAUDE.md`.

## Wymagania

- .NET SDK 8.0.422+ (przypięte w `global.json`)
- SQL Server (lokalny lub zdalny) — baza służy jednocześnie do danych ASP.NET Identity i danych biznesowych

## Uruchomienie lokalne

1. Skonfiguruj connection string `AssetsSquirrelIdentityAccountsDB`:
   ```
   dotnet user-secrets set "ConnectionStrings:AssetsSquirrelIdentityAccountsDB" "Server=...;Database=...;Trusted_Connection=True;TrustServerCertificate=True" --project AssetSquirrelAuthorize.WebApp
   ```
2. Zastosuj migracje EF Core:
   ```
   dotnet ef database update --project "AssetsSquirrel.Plugins\AssetsSquirrel.Plugins.EfCoreSqlServer\AssetsSquirrel.Plugins.EFCoreSqlServer" --startup-project AssetSquirrelAuthorize.WebApp
   ```
3. Uruchom aplikację:
   ```
   dotnet run --project AssetSquirrelAuthorize.WebApp
   ```

Do wysyłki maili potwierdzających konto potrzebna jest konfiguracja SMTP (`Smtp:Username`/`Smtp:Password` w user-secrets, host/port w `appsettings.json`).

## Build i testy

```
dotnet build AssetSquirrel.sln
dotnet test AssetSquirrel.UseCases.Tests/AssetSquirrel.UseCases.Tests.csproj
```

CI (GitHub Actions, `.github/workflows/build-and-test.yml`) uruchamia oba powyższe kroki na każdy push/PR do `master`.