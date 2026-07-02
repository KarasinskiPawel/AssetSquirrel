using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Locations
{
    public class AddLocationsUseCaseTests
    {
        [Fact]
        public async Task AddLocationAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<ILocationRepository>();
            Location? captured = null;
            repository
                .Setup(r => r.AddLocationAsync(It.IsAny<Location>()))
                .Callback<Location>(l => captured = l)
                .ReturnsAsync((Location l) => Result<Location>.Ok(l));

            var useCase = new AddLocationsUseCase(repository.Object);
            var dto = new LocationDto { LocationId = 1, Code = "M100", City = "Łódź", Street = "Piotrkowska 10", MPK = "MPK001", IsActive = true };

            var result = await useCase.AddLocationAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.LocationId, captured.LocationId);
            Assert.Equal(dto.Code, captured.Code);
            Assert.Equal(dto.City, captured.City);
            Assert.Equal(dto.Street, captured.Street);
            repository.Verify(r => r.AddLocationAsync(It.IsAny<Location>()), Times.Once);
        }

        [Fact]
        public async Task AddLocationAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<ILocationRepository>();
            repository.Setup(r => r.AddLocationAsync(It.IsAny<Location>())).ReturnsAsync(Result<Location>.Fail("Database is unavailable."));
            var useCase = new AddLocationsUseCase(repository.Object);
            var result = await useCase.AddLocationAsync(new LocationDto { City = "Stryków", Street = "Targowa 5" });
            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
