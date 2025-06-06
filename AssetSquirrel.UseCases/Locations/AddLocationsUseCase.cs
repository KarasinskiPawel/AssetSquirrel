﻿using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Locations
{
    public class AddLocationsUseCase : IAddLocationsUseCase
    {
        private readonly ILocationRepository locationRepository;

        public AddLocationsUseCase(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        public async Task<bool> AddLocationAsync(LocationDto location)
        {
            return await locationRepository.AddLocationAsync(
                new GenericMapper<Location, LocationDto>().Map(location)
                );
        }
    }
}
