using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Locations
{
    public class ViewLocationsUseCaseTests
    {
        [Fact]
        public async Task GetLocationsAsync_ReturnsMappedDtos()
        {
            var locations = new List<Location>
            {
                new() { LocationId = 1, Code = "M100", City = "Łódź", Street = "Piotrkowska 10", IsActive = true },
                new() { LocationId = 2, Code = "M200", City = "Stryków", Street = "Targowa 5", IsActive = false }
            };

            var repository = new Mock<ILocationRepository>();
            repository
                .Setup(r => r.GetLocationsAsync(It.IsAny<Expression<Func<Location, bool>>>()))
                .ReturnsAsync(locations);

            var useCase = new ViewLocationsUseCase(repository.Object);

            var result = await useCase.GetLocationsAsync(l => l.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.LocationId == 1 && dto.Code == "M100" && dto.City == "Łódź");
            Assert.Contains(result, dto => dto.LocationId == 2 && dto.Street == "Targowa 5");
        }

        [Fact]
        public async Task DeleteLocationAync_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<ILocationRepository>();
            Location? captured = null;
            repository
                .Setup(r => r.DeleteLocationAsync(It.IsAny<Location>()))
                .Callback<Location>(l => captured = l)
                .ReturnsAsync((Location l) => Result<Location>.Ok(l));

            var useCase = new ViewLocationsUseCase(repository.Object);
            var dto = new LocationDto { LocationId = 5, City = "Łódź", Street = "Kilińskiego 20" };

            var result = await useCase.DeleteLocationAync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(5, captured.LocationId);
            repository.Verify(r => r.DeleteLocationAsync(It.IsAny<Location>()), Times.Once);
        }

        [Fact]
        public async Task UpdateLocationAsync_DelegatesToRepositoryUpdateLocationAsync()
        {
            var repository = new Mock<ILocationRepository>();
            repository
                .Setup(r => r.UpdateLocationAsync(It.IsAny<Location>()))
                .ReturnsAsync((Location l) => Result<Location>.Ok(l));

            var useCase = new ViewLocationsUseCase(repository.Object);
            var dto = new LocationDto { LocationId = 7, City = "Stryków", Street = "Targowa 5" };

            var result = await useCase.UpdateLocationAsync(dto);

            Assert.True(result.Success);
            repository.Verify(r => r.UpdateLocationAsync(It.Is<Location>(l => l.LocationId == 7)), Times.Once);
        }
    }
}
