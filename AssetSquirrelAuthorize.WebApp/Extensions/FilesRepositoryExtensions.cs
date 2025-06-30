using AssetsSquirrel.Plugins.InMemory.Files;
using AssetsSquirrel.Plugins.InMemory.Files.Interfaces;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public class FilesRepositoryExtensions
    {
        public static IServiceCollection AddExtensions(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFileManagementRepository, FileManagementRepository>();

            return services;
        }
    }
}
