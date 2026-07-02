using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Manufacturers
{
    public class EditManufactureruseCaseTests
    {
        [Fact]
        public async Task UpdateManufacturerAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<IManufacturersRepository>();
            Manufacturer? captured = null;
            repository
                .Setup(r => r.UpdateManufacturerAsync(It.IsAny<Manufacturer>()))
                .Callback<Manufacturer>(m => captured = m)
                .ReturnsAsync((Manufacturer m) => Result<Manufacturer>.Ok(m));

            var useCase = new EditManufactureruseCase(repository.Object);
            var dto = new ManufacturerDto { ManufacturerId = 2, Name = "HP", Description = "Printers and PCs", IsActive = true };

            var result = await useCase.UpdateManufacturerAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.ManufacturerId, captured.ManufacturerId);
            Assert.Equal(dto.Name, captured.Name);
            repository.Verify(r => r.UpdateManufacturerAsync(It.IsAny<Manufacturer>()), Times.Once);
        }

        [Fact]
        public async Task UpdateManufacturerAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IManufacturersRepository>();
            repository.Setup(r => r.UpdateManufacturerAsync(It.IsAny<Manufacturer>())).ReturnsAsync(Result<Manufacturer>.Fail("Database is unavailable."));
            var useCase = new EditManufactureruseCase(repository.Object);
            var result = await useCase.UpdateManufacturerAsync(new ManufacturerDto { Name = "Lenovo", Description = "Laptops and servers" });
            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
