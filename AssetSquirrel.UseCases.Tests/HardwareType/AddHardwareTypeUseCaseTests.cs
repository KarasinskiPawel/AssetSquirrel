using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.HardwareType
{
    public class AddHardwareTypeUseCaseTests
    {
        [Fact]
        public async Task AddHardwareTypeAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<IHardwareTypeRepository>();
            CoreBusiness.HardwareType? captured = null;
            repository
                .Setup(r => r.AddHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()))
                .Callback<CoreBusiness.HardwareType>(ht => captured = ht)
                .ReturnsAsync((CoreBusiness.HardwareType ht) => Result<CoreBusiness.HardwareType>.Ok(ht));

            var useCase = new AddHardwareTypeUseCase(repository.Object);
            var dto = new HardwareTypeDto { HardwareTypeId = 1, Name = "Laptop", Description = "Przenośny komputer", IsActive = true };

            var result = await useCase.AddHardwareTypeAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.Name, captured.Name);
            Assert.Equal(dto.Description, captured.Description);
            repository.Verify(r => r.AddHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()), Times.Once);
        }

        [Fact]
        public async Task AddHardwareTypeAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IHardwareTypeRepository>();
            repository
                .Setup(r => r.AddHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()))
                .ReturnsAsync(Result<CoreBusiness.HardwareType>.Fail("Database is unavailable."));

            var useCase = new AddHardwareTypeUseCase(repository.Object);

            var result = await useCase.AddHardwareTypeAsync(new HardwareTypeDto { Name = "Monitor" });

            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
