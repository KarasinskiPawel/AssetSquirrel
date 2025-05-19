using AssetSquirrel.UseCases.Extensions;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.Locations;
using AssetSquirrel.WebApp.Components;
using AssetsSquirrel.Plugins.EFCoreSqlServer;
using Microsoft.EntityFrameworkCore;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<AssetsSquirrelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetsSquirrelDB")
        ,sql => sql.MigrationsAssembly("AssetsSquirrel.Plugins.EFCoreSqlServer"));
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

LocationUseCaseExtensions.AddExtensions(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();
