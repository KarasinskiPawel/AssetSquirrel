using AssetSquirrel.UseCases.EquipmentHandover;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrelAuthorize.WebApp.Services;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using AssetsSquirrel.Plugins.InMemory.Files;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class EquipmentHandoverExtension
    {
        public static IServiceCollection AddExtension(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEquipmentHandoverRepository, EquipmentHandoverRepository>();
            services.AddScoped<IEquipmentAssignmentRepository, EquipmentAssignmentRepository>();
            services.AddScoped<IEquipmentHandoverFileManagementRepository, EquipmentHandoverFileManagementRepository>();

            services.AddScoped<IViewEquipmentHandoverUseCase, ViewEquipmentHandoverUseCase>();
            services.AddScoped<IAddEquipmentHandoverUseCase, AddEquipmentHandoverUseCase>();
            services.AddScoped<IEditEquipmentHandoverUseCase, EditEquipmentHandoverUseCase>();
            services.AddScoped<IAddEquipmentHandoverDocumentUseCase, AddEquipmentHandoverDocumentUseCase>();
            services.AddScoped<IEquipmentHandoverPdfGenerator, EquipmentHandoverPdfGenerator>();

            return services;
        }
    }
}
