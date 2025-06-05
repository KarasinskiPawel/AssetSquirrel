using AssetSquirrel.UseCases.Extensions;
using AssetSquirrel.UseCases.Locations;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.WebApp.Components;
using AssetSquirrel.WebApp.Data;
using AssetSquirrel.WebApp.Extensions;
using AssetsSquirrel.Plugins.EFCoreSqlServer;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using BlazorWebApp.Authentication.Components.Account;
using BlazorWebApp.Authentication.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Register services for Identity
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("AssetsSquirrelAccountsDB") ?? throw new InvalidOperationException("Connection string 'AssetsSquirrelAccountsDB' not found.");
builder.Services.AddDbContext<AssetSquirrelUserIdentityContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AssetSquirrelUserIdentityContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
//----------------------------------------------------------------------------------------------------------------

// Add services to the container.
builder.Services.AddDbContextFactory<AssetsSquirrelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsSquirrelDB")
        , sql => sql.MigrationsAssembly("AssetsSquirrel.Plugins.EFCoreSqlServer"));
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        var detailedErrors = builder.Configuration
            .GetSection("CircuitOptions")
            .GetValue<bool>("DetailedErrors");

        options.DetailedErrors = detailedErrors;
    });

//Extensions
LocationUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
DictionaresUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
ErrorsExtensions.AddExtension(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity system / Account Razor Components.
app.MapAdditionalIdentityEndpoints();

app.Run();
