using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.HardwareType;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.HardwareType
{
    public class EditHardwareTypeUseCaseTests
    {
        [Fact]
        public async Task UpdateHardwareTypeAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<IHardwareTypeRepository>();
            CoreBusiness.HardwareType? captured = null;
            repository
                .Setup(r => r.UpdateHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()))
                .Callback<CoreBusiness.HardwareType>(ht => captured = ht)
                .ReturnsAsync((CoreBusiness.HardwareType ht) => Result<CoreBusiness.HardwareType>.Ok(ht));

            var useCase = new EditHardwareTypeUseCase(repository.Object);
            var dto = new HardwareTypeDto { HardwareTypeId = 3, Name = "Drukarka", Description = "Urządzenie drukujące", IsActive = true };

            var result = await useCase.UpdateHardwareTypeAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.HardwareTypeId, captured.HardwareTypeId);
            Assert.Equal(dto.Name, captured.Name);
            repository.Verify(r => r.UpdateHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateHardwareTypeAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IHardwareTypeRepository>();
            repository
                .Setup(r => r.UpdateHardwareTypeAsync(It.IsAny<CoreBusiness.HardwareType>()))
                .ReturnsAsync(Result<CoreBusiness.HardwareType>.Fail("Database is unavailable."));

            var useCase = new EditHardwareTypeUseCase(repository.Object);

            var result = await useCase.UpdateHardwareTypeAsync(new HardwareTypeDto { Name = "Komputer" });

            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
