using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentReturn
{
    public class EditEquipmentReturnUseCaseTests
    {
        [Fact]
        public async Task UpdateEquipmentReturnAsync_ReturnsMappedDto_OnSuccess()
        {
            var repository = new Mock<IEquipmentReturnRepository>();
            repository
                .Setup(r => r.UpdateEquipmentReturnAsync(It.IsAny<AssetSquirrel.CoreBusiness.EquipmentReturn>()))
                .ReturnsAsync((AssetSquirrel.CoreBusiness.EquipmentReturn entity) => Result<AssetSquirrel.CoreBusiness.EquipmentReturn>.Ok(entity));

            var useCase = new EditEquipmentReturnUseCase(repository.Object);

            var dto = new EquipmentReturnDto { EquipmentReturnId = 5, ReturnDocumentNumber = "2026/07/0005", FilePath = "Files/EquipmentReturns/5/signed.pdf" };

            var result = await useCase.UpdateEquipmentReturnAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(5, result.Data!.EquipmentReturnId);
            Assert.Equal("Files/EquipmentReturns/5/signed.pdf", result.Data.FilePath);
        }

        [Fact]
        public async Task UpdateEquipmentReturnAsync_PropagatesFailureFromRepository()
        {
            var repository = new Mock<IEquipmentReturnRepository>();
            repository
                .Setup(r => r.UpdateEquipmentReturnAsync(It.IsAny<AssetSquirrel.CoreBusiness.EquipmentReturn>()))
                .ReturnsAsync(Result<AssetSquirrel.CoreBusiness.EquipmentReturn>.Fail("Update failed."));

            var useCase = new EditEquipmentReturnUseCase(repository.Object);

            var result = await useCase.UpdateEquipmentReturnAsync(new EquipmentReturnDto());

            Assert.False(result.Success);
            Assert.Equal("Update failed.", result.Message);
        }
    }
}
