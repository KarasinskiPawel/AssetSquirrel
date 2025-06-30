using AssetSquirrel.UseCases.EquipmentUseCase;
using AssetSquirrel.UseCases.EquipmentUseCase.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class EquipmentsUseCaseExtensions
    {
        public static IServiceCollection AddExtension(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEquipmentRepository, EquipmentRepository>();

            services.AddScoped<IViewEquipmentUseCase, ViewEquipmentUseCase>();
            services.AddScoped<IAddEquipmentUseCase, AddEquipmentUseCase>();
            services.AddScoped<IEditEquipmentUseCase, EditEquipmentUseCase>();

            return services;
        }
    }
}
