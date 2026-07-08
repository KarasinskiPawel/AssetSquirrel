using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn;
using AssetSquirrel.UseCases.EquipmentReturn.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentReturn
{
    public class AddEquipmentReturnDocumentUseCaseTests
    {
        [Fact]
        public async Task AddEquipmentReturnDocumentAsync_SavesFile_AndDelegatesToEditEquipmentReturnUseCase()
        {
            var fileManagementRepository = new Mock<IEquipmentReturnFileManagementRepository>();
            var editEquipmentReturnUseCase = new Mock<IEditEquipmentReturnUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(Result<bool>.Ok(true));

            editEquipmentReturnUseCase
                .Setup(e => e.UpdateEquipmentReturnAsync(It.IsAny<EquipmentReturnDto>()))
                .ReturnsAsync((EquipmentReturnDto r) => Result<EquipmentReturnDto>.Ok(r));

            var useCase = new AddEquipmentReturnDocumentUseCase(
                fileManagementRepository.Object,
                editEquipmentReturnUseCase.Object);

            var dto = new EquipmentReturnDto { EquipmentReturnId = 10, ReturnDocumentNumber = "2026/07/0010" };

            var result = await useCase.AddEquipmentReturnDocumentAsync(dto, "signed.pdf", "application/pdf", new MemoryStream());

            Assert.True(result.Success);
            fileManagementRepository.Verify(
                f => f.AddNewFile(10, "signed.pdf", "application/pdf", It.IsAny<Stream>()),
                Times.Once);
            editEquipmentReturnUseCase.Verify(e => e.UpdateEquipmentReturnAsync(It.IsAny<EquipmentReturnDto>()), Times.Once);
        }

        [Fact]
        public async Task AddEquipmentReturnDocumentAsync_ReturnsFailureWithMessage_WhenFileSaveFails()
        {
            var fileManagementRepository = new Mock<IEquipmentReturnFileManagementRepository>();
            var editEquipmentReturnUseCase = new Mock<IEditEquipmentReturnUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(Result<bool>.Fail("Disk write failed."));

            var useCase = new AddEquipmentReturnDocumentUseCase(
                fileManagementRepository.Object,
                editEquipmentReturnUseCase.Object);

            var dto = new EquipmentReturnDto { EquipmentReturnId = 11, ReturnDocumentNumber = "2026/07/0011" };

            var result = await useCase.AddEquipmentReturnDocumentAsync(dto, "signed.pdf", "application/pdf", Stream.Null);

            Assert.False(result.Success);
            Assert.Equal("Disk write failed.", result.Message);
            editEquipmentReturnUseCase.Verify(e => e.UpdateEquipmentReturnAsync(It.IsAny<EquipmentReturnDto>()), Times.Never);
        }

        [Fact]
        public async Task AddEquipmentReturnDocumentAsync_SetsFilePathAndUploadDate_BeforeCallingEditUseCase()
        {
            var fileManagementRepository = new Mock<IEquipmentReturnFileManagementRepository>();
            var editEquipmentReturnUseCase = new Mock<IEditEquipmentReturnUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(Result<bool>.Ok(true));

            EquipmentReturnDto? captured = null;
            editEquipmentReturnUseCase
                .Setup(e => e.UpdateEquipmentReturnAsync(It.IsAny<EquipmentReturnDto>()))
                .Callback<EquipmentReturnDto>(r => captured = r)
                .ReturnsAsync((EquipmentReturnDto r) => Result<EquipmentReturnDto>.Ok(r));

            var useCase = new AddEquipmentReturnDocumentUseCase(
                fileManagementRepository.Object,
                editEquipmentReturnUseCase.Object);

            var dto = new EquipmentReturnDto { EquipmentReturnId = 12, ReturnDocumentNumber = "2026/07/0012" };
            var beforeCall = DateTime.Now;

            var result = await useCase.AddEquipmentReturnDocumentAsync(dto, "signed.pdf", "application/pdf", new MemoryStream());

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(Path.Combine("Files", "EquipmentReturns", "12", "signed.pdf"), captured.FilePath);
            Assert.NotNull(captured.UploadDate);
            Assert.True(captured.UploadDate >= beforeCall);
        }
    }
}
