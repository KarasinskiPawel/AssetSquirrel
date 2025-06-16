using AssetSquirrel.UseCases.Employees;
using AssetSquirrel.UseCases.Employees.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class EmployeesUseCaseExtensions
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IViewEmployeesUseCase, ViewEmployeesUseCase>();
            services.AddScoped<IAddEmployeeUseCase, AddEmployeeUseCase>();
            services.AddScoped<IEditEmployeeUseCase, EditEmployeeUseCase>();

            return services;
        }
    }
}
