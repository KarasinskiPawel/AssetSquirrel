using AssetSquirrel.UseCases.EquipmentHandover;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class EquipmentHandoverExtension
    {
        public static IServiceCollection AddExtension(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEquipmentHandoverRepository, EquipmentHandoverRepository>();

            services.AddScoped<IViewEquipmentHandover, ViewEquipmentHandover>();

            return services;
        }
    }
}
