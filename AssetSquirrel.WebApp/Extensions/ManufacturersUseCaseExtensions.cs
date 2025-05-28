using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrel.WebApp.Extensions
{
    public static class ManufacturersUseCaseExtensions
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IManufacturersRepository, ManufacturersRepository>();
            
            services.AddScoped<IViewManufacture>

            return services;
        }
    }
}
