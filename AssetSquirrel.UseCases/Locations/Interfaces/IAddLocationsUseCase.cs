﻿using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.Locations.Interfaces
{
    public interface IAddLocationsUseCase
    {
        Task<bool> AddLocationAsync(LocationDto location);
    }
}