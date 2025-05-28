using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers;
using AssetSquirrel.UseCases.Suppilers.Interfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrel.WebApp.Extensions
{
    public static class DictionaresUseCaseExtensions
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISuppilersRepository, SuppilersRepository>();

            services.AddScoped<IViewSuppilersUseCase, ViewSuppilersUseCase>();
            services.AddScoped<IAddSuppilerUseCase, AddSuppilerUseCase>();
            services.AddScoped<IEditSuppilerUseCase, EditSuppilerUseCase>();

            return services;
        }
    }
}
