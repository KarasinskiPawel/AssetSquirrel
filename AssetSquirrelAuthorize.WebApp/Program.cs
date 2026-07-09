using AssetSquirrelAuthorize.UseCases.Extensions;
using AssetSquirrelAuthorize.WebApp;
using AssetSquirrelAuthorize.WebApp.Components;
using AssetSquirrelAuthorize.WebApp.Components.Account;
using AssetSquirrelAuthorize.WebApp.Extensions;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetsSquirrel.CoreBusiness;
using AssetsSquirrel.Plugins.EFCoreSqlServer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Persist Data Protection keys outside the deployed app folder so they survive IIS app-pool
// recycles/restarts and redeploys. Without this, every recycle silently invalidates the
// antiforgery tokens embedded in already-open Blazor Server circuits (see docs/deployment-iis.md).
#pragma warning disable CA1416 // this app is only ever deployed to Windows/IIS, see docs/deployment-iis.md
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "AssetSquirrel", "DataProtection-Keys")))
    .ProtectKeysWithDpapi(protectToLocalMachine: true);
#pragma warning restore CA1416

builder.Services.AddExceptionHandler<AntiforgeryTokenExceptionHandler>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContextFactory<AssetsSquirrelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsSquirrelIdentityAccountsDB")
        ?? throw new InvalidOperationException("Connection string 'AssetsSquirrelIdentityAccountsDB' not found. On production, it must be set in appsettings.Production.json (see docs/deployment-iis.md).")
        , sql => sql.MigrationsAssembly("AssetsSquirrel.Plugins.EFCoreSqlServer"));
});

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AssetsSquirrelContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("AssetsSquirrelAccountsDB") ?? throw new InvalidOperationException("Connection string 'AssetsSquirrelAccountsDB' not found.")
//    ));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IEmailSender<ApplicationUser>, SmtpEmailSender>();

builder.Services.Configure<CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

//Extensions
FilesRepositoryExtensions.AddExtensions(builder.Services, builder.Configuration);
LocationUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
DictionaresUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
EmployeesUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
EquipmentsUseCaseExtensions.AddExtension(builder.Services, builder.Configuration);
ErrorsExtensions.AddExtension(builder.Services, builder.Configuration);
InvoicesUseCaseExtensions.AddExtension(builder.Services, builder.Configuration);
EquipmentHandoverExtension.AddExtension(builder.Services, builder.Configuration);
EquipmentReturnExtension.AddExtension(builder.Services, builder.Configuration);
EquipmentAssignmentExtension.AddExtension(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // Registered with a fallback path so AntiforgeryTokenExceptionHandler can redirect
    // stale-token requests (e.g. Logout from a page left open across a Data Protection key
    // rotation) to Login; anything else it doesn't handle falls back to the generic "/Error" page.
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandlingPath = "/Error",
        CreateScopeForErrors = true,
    });
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapGet("/api/equipmenthandover/{id:int}/pdf", async (int id, IViewEquipmentHandoverUseCase viewEquipmentHandoverUseCase, IEquipmentHandoverPdfGenerator pdfGenerator) =>
{
    var handover = (await viewEquipmentHandoverUseCase.GetEquipmentHandoverAsync(h => h.EquipmentHandoverId == id)).FirstOrDefault();

    if (handover is null)
    {
        return Results.NotFound();
    }

    var pdfBytes = pdfGenerator.Generate(handover);
    var downloadName = $"{handover.HandoverDocumentNumber.Replace('/', '-')}.pdf";

    return Results.File(pdfBytes, "application/pdf", downloadName);
});

app.MapGet("/api/equipmentreturn/{id:int}/pdf", async (int id, IViewEquipmentReturnUseCase viewEquipmentReturnUseCase, IEquipmentReturnPdfGenerator pdfGenerator) =>
{
    var equipmentReturn = (await viewEquipmentReturnUseCase.GetEquipmentReturnsAsync(r => r.EquipmentReturnId == id)).FirstOrDefault();

    if (equipmentReturn is null)
    {
        return Results.NotFound();
    }

    var pdfBytes = pdfGenerator.Generate(equipmentReturn);
    var downloadName = $"{equipmentReturn.ReturnDocumentNumber.Replace('/', '-')}.pdf";

    return Results.File(pdfBytes, "application/pdf", downloadName);
});

app.MapGet("/whoami", (HttpContext context) => new
{
    Name = context.User.Identity?.Name,
    IsAuthenticated = context.User.Identity?.IsAuthenticated,
    AuthType = context.User.Identity?.AuthenticationType,
    Claims = context.User.Claims.Select(c => new { c.Type, c.Value })
});

using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AssetsSquirrelContext>>();
    await using var dbContext = await dbContextFactory.CreateDbContextAsync();
    await dbContext.Database.MigrateAsync();
}

// Identity role bootstrap: ensure Admin/View roles exist and grandfather
// any pre-existing account with no role into Admin (idempotent).
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { "Admin", "View" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    foreach (var user in userManager.Users.ToList())
    {
        if ((await userManager.GetRolesAsync(user)).Count == 0)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

app.Run();
