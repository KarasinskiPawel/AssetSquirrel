using AssetSquirrel.UseCases.EquipmentReturn;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrelAuthorize.WebApp.Services;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using AssetsSquirrel.Plugins.InMemory.Files;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class EquipmentReturnExtension
    {
        public static IServiceCollection AddExtension(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEquipmentReturnRepository, EquipmentReturnRepository>();
            services.AddScoped<IEquipmentReturnFileManagementRepository, EquipmentReturnFileManagementRepository>();

            services.AddScoped<IViewEquipmentReturnUseCase, ViewEquipmentReturnUseCase>();
            services.AddScoped<IAddEquipmentReturnUseCase, AddEquipmentReturnUseCase>();
            services.AddScoped<IEditEquipmentReturnUseCase, EditEquipmentReturnUseCase>();
            services.AddScoped<IAddEquipmentReturnDocumentUseCase, AddEquipmentReturnDocumentUseCase>();
            services.AddScoped<IEquipmentReturnPdfGenerator, EquipmentReturnPdfGenerator>();

            return services;
        }
    }
}
