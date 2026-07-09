# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

AssetSquirrel is an internal IT asset-management app (equipment, employees, handovers, invoices, locations, manufacturers, suppliers) built as a Clean-Architecture-flavored .NET 8 solution with a Blazor Server front end.

## Build & test

```
dotnet build AssetSquirrel.sln
dotnet test AssetSquirrel.sln
```

`global.json` pins the SDK to 8.0.422 (`rollForward: latestFeature`). CI (`.github/workflows/build-and-test.yml`, GitHub Actions) runs both commands above on every push/PR to `master`.

Unit tests live in `AssetSquirrel.UseCases.Tests` (xUnit + Moq), one folder per feature area, covering every UseCase class — mock the relevant `PluginInterfaces` repository interface, verify entity↔DTO mapping and `Result<T>` propagation. Follow this pattern for new UseCase code. `AssetSquirrelAuthorize.WebApp.Tests` (xUnit) covers standalone WebApp-project classes that don't fit the UseCase pattern (e.g. `AntiforgeryTokenExceptionHandler`) — it references the WebApp project directly rather than mocking a plugin interface.

EF Core migrations live in the `AssetsSquirrel.Plugins.EFCoreSqlServer` project, DbContext is `AssetsSquirrelContext`. To add a migration, run from repo root with the WebApp as startup project:

```
dotnet ef migrations add <Name> --project "AssetsSquirrel.Plugins\AssetsSquirrel.Plugins.EfCoreSqlServer\AssetsSquirrel.Plugins.EFCoreSqlServer" --startup-project AssetSquirrelAuthorize.WebApp
```

## The web app

`AssetSquirrelAuthorize.WebApp` is the only web app project (the earlier abandoned scaffold, `AssetSquirrel.WebApp`, has been deleted from the repo). It wires up every use case and unifies ASP.NET Identity with business data in one `DbContext`.

## Architecture

Layering (dependencies point inward/upward through interfaces):

```
AssetSquirrel.CoreBusiness   (entities + DTOs, no persistence deps)
        ^
AssetSquirrel.UseCases       (business logic + PluginInterfaces for DIP)
        ^
Plugins (implement PluginInterfaces):
  - AssetsSquirrel.Plugins.EFCoreSqlServer  (real DB: EF Core + SQL Server)
  - AssetsSquirrel.Plugins.InMemory         (NOT an in-memory store — only local-disk
                                              file storage for invoice attachments)
        ^
AssetSquirrelAuthorize.WebApp (composition root, Blazor Server UI)
```

- **CoreBusiness**: flat POCO entities (`Equipment`, `Employee`, `EquipmentHandover`, `Invoice`, `Location`, `Manufacturer`, `Suppiler`, etc.) with DataAnnotations, plus a `Dto/` folder that flattens navigation properties (e.g. `EquipmentDto.ManufacturerName`) for UI binding. `ApplicationUser : IdentityUser` also lives here so the domain layer owns the Identity/business link (`Equipment.UserId`, `EquipmentAssignment.UserId`). `Result.cs` defines `Result<T>` (see below).
- **UseCases**: one folder per feature (`Employees`, `EquipmentUseCase`, `EquipmentHandover`, `Invoices`, `Locations`, `Manufacturers`, `Suppilers`, `HardwareType`), each with `Add`/`Edit`/`View` use-case classes and matching interfaces. Use cases take `PluginInterfaces` repository interfaces via constructor injection — this is the dependency-inversion boundary the plugins implement. Entity↔DTO mapping uses Mapster's `.Adapt<T>()` extension method directly (no wrapper class).
- **Plugins**: real project paths are double-nested with mismatched casing, e.g. the EF Core plugin's actual csproj is at `AssetsSquirrel.Plugins\AssetsSquirrel.Plugins.EfCoreSqlServer\AssetsSquirrel.Plugins.EFCoreSqlServer\AssetsSquirrel.Plugins.EFCoreSqlServer.csproj` (outer folder lowercase `Ef`, inner real project uppercase `EF`). Same double-nesting applies to the InMemory plugin.
  - `AssetsSquirrelContext : IdentityDbContext<ApplicationUser>` holds both Identity tables and every business `DbSet` — one database for auth and business data.
  - Repositories take `IDbContextFactory<AssetsSquirrelContext>` (not a scoped `DbContext`) plus `IErrorsRepository`.
  - `AssetsSquirrel.Plugins.InMemory` — despite the name, not an in-memory store; it only implements `IFileManagementRepository` (local-disk file storage for invoice attachments under `wwwroot/Files/Invoices`).
- **WebApp**: Blazor Server (`AddInteractiveServerComponents` / `AddInteractiveServerRenderMode`, not WebAssembly). DI wiring is manual, in per-feature static `Extensions/*.cs` classes (`EmployeesUseCaseExtensions.AddExtensions(services, config)`, etc.) called from `Program.cs` — no reflection-based auto-registration. `Components/Pages` has one feature folder per area (`Employees`, `Equipment`, `EquipmentHandover`, `EquipmentReturn`, `Invoices`, `Locations`, `Dictionares`), each usually with an index Razor plus `*AddDialogBox.razor` / `*EditDialogBox.razor` modal components built on the shared `Components/Template/DialogBox.razor`. `Components/Account/*` is the standard `dotnet new blazor --auth Individual` Identity scaffold (Login/Register/2FA/Manage pages).

### Error handling — `Result<T>`

`AssetSquirrel.CoreBusiness.Result<T>` (`Success` bool, `Message` string?, `Data` T?, plus `.Select<TOut>()` to remap `Data` while preserving `Success`/`Message`) is the return type for every mutating method (`Add/Update/Delete...Async`) across `PluginInterfaces`, plugin repository implementations, and UseCase interfaces/implementations — `Result<Entity>` at the repository layer, `Result<Dto>` at the UseCase layer (mapped via `.Select(e => e.Adapt<Dto>())`). This now also covers `IFileManagementRepository`/`IEquipmentHandoverFileManagementRepository`/`IEquipmentReturnFileManagementRepository` and `IErrorsRepository` (all `Result<bool>`) — no repository is exempt from the convention anymore. On exception, the repository catches it, logs it via `IErrorsRepository.AddErrorAsync(service, class, method, exception)`, and returns `Result<T>.Fail(ex.Message)`. Read (`Get*Async`) methods are unaffected — they still return the list/DTO directly, not wrapped in `Result<T>`. In the Blazor UI, dialog components (`*AddDialogBox.razor`/`*EditDialogBox.razor`) expose `EventCallback<Result<TDto>> OnSave` and invoke it with the full `Result<TDto>` — both the use-case result and any client-side validation failure (synthesized via `Result<TDto>.Fail(...)`). Parent-page handlers show the outcome via the shared `AssetSquirrelAuthorize.WebApp.Extensions.ResultToastExtensions.ShowToastAsync` helper, which toasts `Result.Message` when present and falls back to a per-operation-kind message ("Błąd zapisu" for Add, "Błąd aktualizacji" for Edit, "Błąd operacji" otherwise) when `Message` is null.

### Identity

ASP.NET Core Identity (`AddIdentityCore<ApplicationUser>`, cookie auth via `AddIdentityCookies()`, `IdentityRevalidatingAuthenticationStateProvider` for Blazor Server circuits). `IEmailSender<ApplicationUser>` is `SmtpEmailSender` (`Components/Account/SmtpEmailSender.cs`, plain `System.Net.Mail.SmtpClient`) — real confirmation/reset emails are sent. SMTP host/port/from live in `appsettings.json` (`Smtp` section); username/password are set via `dotnet user-secrets` (never commit them).

Connection string name: `AssetsSquirrelIdentityAccountsDB` (set in `appsettings.Development.json` / user secrets, not committed with real values).

## Repo-specific naming quirks (real, not typos to silently fix)

These are consistent throughout the codebase — match them when touching related code, don't "correct" them without being asked:

- `AssetSquirrel` vs `AssetsSquirrel` (extra "s") is inconsistent across the repo: solution root is `AssetSquirrel`, but the Plugins projects/solution folder and some namespaces (e.g. `ApplicationUser`'s `namespace AssetsSquirrel.CoreBusiness`) use `AssetsSquirrel`.
- **"Suppiler(s)"** is used everywhere instead of "Supplier(s)" — entity, DTO, repository, use cases, UI folder all consistently misspelled this way.
- **"Dictionares"** (missing "i") instead of "Dictionaries" — the reference-data admin page folder/extension class.
- `EquipmetAssignment` (missing "n") — a `Components/Pages` folder name.
- Use-case casing drift on a few classes: `AddHardwareTypeuseCase`, `AddManufacturerUserCase` (not "UseCase"), `EditManufactureruseCase`.
