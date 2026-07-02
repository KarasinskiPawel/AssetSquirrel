using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Manufacturers
{
    public class AddManufacturerUserCaseTests
    {
        [Fact]
        public async Task AddManufacturerAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<IManufacturersRepository>();
            Manufacturer? captured = null;
            repository
                .Setup(r => r.AddManufacturerAsync(It.IsAny<Manufacturer>()))
                .Callback<Manufacturer>(m => captured = m)
                .ReturnsAsync((Manufacturer m) => Result<Manufacturer>.Ok(m));

            var useCase = new AddManufacturerUserCase(repository.Object);
            var dto = new ManufacturerDto { ManufacturerId = 1, Name = "Asus", Description = "Laptops and motherboards", IsActive = true };

            var result = await useCase.AddManufacturerAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.Name, captured.Name);
            Assert.Equal(dto.Description, captured.Description);
            repository.Verify(r => r.AddManufacturerAsync(It.IsAny<Manufacturer>()), Times.Once);
        }

        [Fact]
        public async Task AddManufacturerAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IManufacturersRepository>();
            repository.Setup(r => r.AddManufacturerAsync(It.IsAny<Manufacturer>())).ReturnsAsync(Result<Manufacturer>.Fail("Database is unavailable."));
            var useCase = new AddManufacturerUserCase(repository.Object);
            var result = await useCase.AddManufacturerAsync(new ManufacturerDto { Name = "Acer", Description = "Consumer electronics" });
            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
