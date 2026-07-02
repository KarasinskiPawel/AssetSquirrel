using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.HardwareType
{
    public class ViewHardwareTypeUseCaseTests
    {
        [Fact]
        public async Task GetHardwareTypesAsync_ReturnsMappedDtos()
        {
            var hardwareTypes = new List<CoreBusiness.HardwareType>
            {
                new() { HardwareTypeId = 1, Name = "Komputer", Description = "Stacja robocza", IsActive = true },
                new() { HardwareTypeId = 2, Name = "Monitor", Description = "Wyświetlacz", IsActive = false }
            };

            var repository = new Mock<IHardwareTypeRepository>();
            repository
                .Setup(r => r.GetHardwareTypesAsync(It.IsAny<Expression<Func<CoreBusiness.HardwareType, bool>>>()))
                .ReturnsAsync(hardwareTypes);

            var useCase = new ViewHardwareTypeUseCase(repository.Object);

            var result = await useCase.GetHardwareTypesAsync(ht => ht.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.HardwareTypeId == 1 && dto.Name == "Komputer" && dto.Description == "Stacja robocza");
            Assert.Contains(result, dto => dto.HardwareTypeId == 2 && dto.Name == "Monitor");
        }

        [Fact]
        public async Task UpdateHardwareType_DelegatesToRepositoryUpdateHardwareTypeAsync()
        {
            var repository = new Mock<IHardwareTypeRepository>();
            repository
                .Setup(r => r.UpdateHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()))
                .ReturnsAsync((CoreBusiness.HardwareType ht) => Result<CoreBusiness.HardwareType>.Ok(ht));

            var useCase = new ViewHardwareTypeUseCase(repository.Object);
            var dto = new HardwareTypeDto { HardwareTypeId = 4, Name = "Laptop", Description = "Przenośny komputer" };

            var result = await useCase.UpdateHardwareType(dto);

            Assert.True(result.Success);
            repository.Verify(r => r.UpdateHardwareTypeAsync(It.Is<CoreBusiness.HardwareType>(ht => ht.HardwareTypeId == 4)), Times.Once);
        }

        [Fact]
        public async Task DeleteHardwareTypeAsync_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<IHardwareTypeRepository>();
            CoreBusiness.HardwareType? captured = null;
            repository
                .Setup(r => r.DeleteHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()))
                .Callback<CoreBusiness.HardwareType>(ht => captured = ht)
                .ReturnsAsync((CoreBusiness.HardwareType ht) => Result<CoreBusiness.HardwareType>.Ok(ht));

            var useCase = new ViewHardwareTypeUseCase(repository.Object);
            var dto = new HardwareTypeDto { HardwareTypeId = 5, Name = "Drukarka" };

            var result = await useCase.DeleteHardwareTypeAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(5, captured.HardwareTypeId);
            repository.Verify(r => r.DeleteHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()), Times.Once);
        }
    }
}
