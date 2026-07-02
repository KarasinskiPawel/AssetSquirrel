using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.Locations;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Locations
{
    public class EditLocationUseCaseTests
    {
        [Fact]
        public async Task UpdateLocationAsync_MapsDtoToEntity_AndDelegatesToUpdateLocationAsync()
        {
            var repository = new Mock<ILocationRepository>();
            Location? captured = null;
            repository
                .Setup(r => r.UpdateLocationAsync(It.IsAny<Location>()))
                .Callback<Location>(l => captured = l)
                .ReturnsAsync((Location l) => Result<Location>.Ok(l));

            var useCase = new EditLocationUseCase(repository.Object);
            var dto = new LocationDto
            {
                LocationId = 3,
                Code = "M103",
                City = "Łódź",
                Street = "Kilińskiego 20",
                Email = "magazyn.lodz@example.com",
                PhoneNumber = "123456789",
                IsActive = false
            };

            var result = await useCase.UpdateLocationAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.LocationId, captured.LocationId);
            Assert.Equal(dto.PhoneNumber, captured.PhoneNumber);
            Assert.Equal(dto.IsActive, captured.IsActive);
            repository.Verify(r => r.UpdateLocationAsync(It.IsAny<Location>()), Times.Once);
        }

        [Fact]
        public async Task UpdateLocationAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<ILocationRepository>();
            repository
                .Setup(r => r.UpdateLocationAsync(It.IsAny<Location>()))
                .ReturnsAsync(Result<Location>.Fail("Location not found."));

            var useCase = new EditLocationUseCase(repository.Object);
            var dto = new LocationDto { LocationId = 3, City = "Łódź", Street = "Kilińskiego 20" };

            var result = await useCase.UpdateLocationAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Location not found.", result.Message);
        }
    }
}
