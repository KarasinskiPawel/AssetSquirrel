using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AssetSquirrel.UseCases.Locations;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSquirrelAuthorize.UseCases.Extensions
{
    public static class LocationUseCaseExtensions
    {
        public static IServiceCollection AddExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILocationRepository, LocationRepository>();

            services.AddScoped<IViewLocationsUseCase, ViewLocationsUseCase>();
            services.AddScoped<IAddLocationsUseCase, AddLocationsUseCase>();
            services.AddScoped<IEditLocationUseCase, EditLocationUseCase>();

            return services;
        }
    }
}
