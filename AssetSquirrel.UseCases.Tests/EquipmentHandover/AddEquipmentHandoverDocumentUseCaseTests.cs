using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover;
using AssetSquirrel.UseCases.EquipmentHandover.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentHandover
{
    public class AddEquipmentHandoverDocumentUseCaseTests
    {
        [Fact]
        public async Task AddEquipmentHandoverDocumentAsync_SavesFile_AndDelegatesToEditEquipmentHandoverUseCase()
        {
            var fileManagementRepository = new Mock<IEquipmentHandoverFileManagementRepository>();
            var editEquipmentHandoverUseCase = new Mock<IEditEquipmentHandoverUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(true);

            editEquipmentHandoverUseCase
                .Setup(e => e.UpdateEquipmentHandoverAsync(It.IsAny<EquipmentHandoverDto>()))
                .ReturnsAsync((EquipmentHandoverDto h) => Result<EquipmentHandoverDto>.Ok(h));

            var useCase = new AddEquipmentHandoverDocumentUseCase(
                fileManagementRepository.Object,
                editEquipmentHandoverUseCase.Object);

            var dto = new EquipmentHandoverDto { EquipmentHandoverId = 10, HandoverDocumentNumber = "2026/07/0010" };

            var result = await useCase.AddEquipmentHandoverDocumentAsync(dto, "signed.pdf", "application/pdf", new MemoryStream());

            Assert.True(result.Success);
            fileManagementRepository.Verify(
                f => f.AddNewFile(10, "signed.pdf", "application/pdf", It.IsAny<Stream>()),
                Times.Once);
            editEquipmentHandoverUseCase.Verify(e => e.UpdateEquipmentHandoverAsync(It.IsAny<EquipmentHandoverDto>()), Times.Once);
        }

        [Fact]
        public async Task AddEquipmentHandoverDocumentAsync_ReturnsFailureWithMessage_WhenFileSaveFails()
        {
            var fileManagementRepository = new Mock<IEquipmentHandoverFileManagementRepository>();
            var editEquipmentHandoverUseCase = new Mock<IEditEquipmentHandoverUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(false);

            var useCase = new AddEquipmentHandoverDocumentUseCase(
                fileManagementRepository.Object,
                editEquipmentHandoverUseCase.Object);

            var dto = new EquipmentHandoverDto { EquipmentHandoverId = 11, HandoverDocumentNumber = "2026/07/0011" };

            var result = await useCase.AddEquipmentHandoverDocumentAsync(dto, "signed.pdf", "application/pdf", Stream.Null);

            Assert.False(result.Success);
            Assert.Equal("Failed to save the equipment handover document file.", result.Message);
            editEquipmentHandoverUseCase.Verify(e => e.UpdateEquipmentHandoverAsync(It.IsAny<EquipmentHandoverDto>()), Times.Never);
        }

        [Fact]
        public async Task AddEquipmentHandoverDocumentAsync_SetsFilePathUploadDateAndIsPosted_BeforeCallingEditUseCase()
        {
            var fileManagementRepository = new Mock<IEquipmentHandoverFileManagementRepository>();
            var editEquipmentHandoverUseCase = new Mock<IEditEquipmentHandoverUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(true);

            EquipmentHandoverDto? captured = null;
            editEquipmentHandoverUseCase
                .Setup(e => e.UpdateEquipmentHandoverAsync(It.IsAny<EquipmentHandoverDto>()))
                .Callback<EquipmentHandoverDto>(h => captured = h)
                .ReturnsAsync((EquipmentHandoverDto h) => Result<EquipmentHandoverDto>.Ok(h));

            var useCase = new AddEquipmentHandoverDocumentUseCase(
                fileManagementRepository.Object,
                editEquipmentHandoverUseCase.Object);

            var dto = new EquipmentHandoverDto { EquipmentHandoverId = 12, HandoverDocumentNumber = "2026/07/0012", IsPosted = false };
            var beforeCall = DateTime.Now;

            var result = await useCase.AddEquipmentHandoverDocumentAsync(dto, "signed.pdf", "application/pdf", new MemoryStream());

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(Path.Combine("Files", "EquipmentHandovers", "12", "signed.pdf"), captured.FilePath);
            Assert.NotNull(captured.UploadDate);
            Assert.True(captured.UploadDate >= beforeCall);
            Assert.True(captured.IsPosted);
        }
    }
}
