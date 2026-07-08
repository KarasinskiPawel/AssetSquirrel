using AssetSquirrel.UseCases.EquipmentAssignment;
using AssetSquirrel.UseCases.EquipmentAssignment.Interfaces;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class EquipmentAssignmentExtension
    {
        public static IServiceCollection AddExtension(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IViewEquipmentAssignmentUseCase, ViewEquipmentAssignmentUseCase>();

            return services;
        }
    }
}
