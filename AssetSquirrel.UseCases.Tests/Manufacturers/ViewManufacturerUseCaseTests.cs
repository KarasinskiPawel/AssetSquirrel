using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Manufacturers;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Manufacturers
{
    public class ViewManufacturerUseCaseTests
    {
        [Fact]
        public async Task GetManufacturersAsync_ReturnsMappedDtos()
        {
            var manufacturers = new List<Manufacturer>
            {
                new() { ManufacturerId = 1, Name = "Lenovo", Description = "Laptops and servers", IsActive = true },
                new() { ManufacturerId = 2, Name = "Cisco", Description = "Networking equipment", IsActive = false }
            };

            var repository = new Mock<IManufacturersRepository>();
            repository
                .Setup(r => r.GetManufacturersAsync(It.IsAny<Expression<Func<Manufacturer, bool>>>()))
                .ReturnsAsync(manufacturers);

            var useCase = new ViewManufacturerUseCase(repository.Object);

            var result = await useCase.GetManufacturersAsync(m => m.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.ManufacturerId == 1 && dto.Name == "Lenovo" && dto.Description == "Laptops and servers");
            Assert.Contains(result, dto => dto.ManufacturerId == 2 && dto.Name == "Cisco");
        }

        [Fact]
        public async Task UpdateManufacturer_DelegatesToRepositoryUpdateManufacturerAsync()
        {
            var repository = new Mock<IManufacturersRepository>();
            repository
                .Setup(r => r.UpdateManufacturerAsync(It.IsAny<Manufacturer>()))
                .ReturnsAsync((Manufacturer m) => Result<Manufacturer>.Ok(m));

            var useCase = new ViewManufacturerUseCase(repository.Object);
            var dto = new ManufacturerDto { ManufacturerId = 3, Name = "Asus", Description = "Laptops and motherboards" };

            var result = await useCase.UpdateManufacturer(dto);

            Assert.True(result.Success);
            repository.Verify(r => r.UpdateManufacturerAsync(It.Is<Manufacturer>(m => m.ManufacturerId == 3)), Times.Once);
        }

        [Fact]
        public async Task Deletemanufacturer_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<IManufacturersRepository>();
            Manufacturer? captured = null;
            repository
                .Setup(r => r.DeleteManufacturerAsync(It.IsAny<Manufacturer>()))
                .Callback<Manufacturer>(m => captured = m)
                .ReturnsAsync((Manufacturer m) => Result<Manufacturer>.Ok(m));

            var useCase = new ViewManufacturerUseCase(repository.Object);
            var dto = new ManufacturerDto { ManufacturerId = 4, Name = "Acer", Description = "Consumer electronics" };

            var result = await useCase.Deletemanufacturer(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(4, captured.ManufacturerId);
            repository.Verify(r => r.DeleteManufacturerAsync(It.IsAny<Manufacturer>()), Times.Once);
        }
    }
}
