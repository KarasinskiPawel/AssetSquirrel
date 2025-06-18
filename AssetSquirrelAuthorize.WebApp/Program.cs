using AssetSquirrelAuthorize.UseCases.Extensions;
using AssetSquirrelAuthorize.WebApp.Components;
using AssetSquirrelAuthorize.WebApp.Components.Account;
using AssetSquirrelAuthorize.WebApp.Extensions;
using AssetsSquirrel.CoreBusiness;
using AssetsSquirrel.Plugins.EFCoreSqlServer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContextFactory<AssetsSquirrelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsSquirrelIdentityAccountsDB")
        , sql => sql.MigrationsAssembly("AssetsSquirrel.Plugins.EFCoreSqlServer"));
});

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AssetsSquirrelContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("AssetsSquirrelAccountsDB") ?? throw new InvalidOperationException("Connection string 'AssetsSquirrelAccountsDB' not found.")
//    ));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.Configure<CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

//Extensions
LocationUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
DictionaresUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
EmployeesUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);
EquipmentsUseCaseExtensions.AddExtension(builder.Services, builder.Configuration);
ErrorsExtensions.AddExtension(builder.Services, builder.Configuration);
InvoicesUseCaseExtensions.AddExtension(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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

app.Run();
