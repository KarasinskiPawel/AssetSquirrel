using AssetSquirrel.UseCases.HardwareType;
using AssetSquirrel.UseCases.HardwareType.Interfaces;
using AssetSquirrel.UseCases.Manufacturers;
using AssetSquirrel.UseCases.Manufacturers.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers;
using AssetSquirrel.UseCases.Suppilers.Interfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class DictionaresUseCaseExtensions
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISuppilersRepository, SuppilersRepository>();
            services.AddScoped<IManufacturersRepository, ManufacturersRepository>();
            services.AddScoped<IHardwareTypeRepository, HardwareTypeRepository>();

            services.AddScoped<IViewSuppilersUseCase, ViewSuppilersUseCase>();
            services.AddScoped<IAddSuppilerUseCase, AddSuppilerUseCase>();
            services.AddScoped<IEditSuppilerUseCase, EditSuppilerUseCase>();

            services.AddScoped<IViewManufacturerUseCase, ViewManufacturerUseCase>();
            services.AddScoped<IAddManufacturerUserCase, AddManufacturerUserCase>();
            services.AddScoped<IEditManufactureruseCase, EditManufactureruseCase>();

            services.AddScoped<IViewHardwareTypeUseCase, ViewHardwareTypeUseCase>();
            services.AddScoped<IAddHardwareTypeUseCase, AddHardwareTypeUseCase>();
            services.AddScoped<IEditHardwareTypeUseCase, EditHardwareTypeUseCase>();

            return services;
        }
    }
}
