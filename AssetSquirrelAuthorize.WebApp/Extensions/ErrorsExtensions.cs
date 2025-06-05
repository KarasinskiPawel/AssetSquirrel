using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class ErrorsExtensions
    {
        public static IServiceCollection AddExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IErrorsRepository, ErrorsRepository>();

            return services;
        }
    }
}
