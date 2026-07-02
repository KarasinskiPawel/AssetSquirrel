using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices;
using AssetSquirrel.UseCases.Invoices.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Invoices
{
    public class AddInvoiceDocumentUseCaseTests
    {
        [Fact]
        public async Task AddInvoiceDocumentAsync_SavesFile_AndDelegatesToEditInvoiceUseCase()
        {
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var viewInvoicesUseCase = new Mock<IViewInvoicesUseCase>();
            var fileManagementRepository = new Mock<IFileManagementRepository>();
            var editInvoiceUseCase = new Mock<IEditInvoiceUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(true);

            editInvoiceUseCase
                .Setup(e => e.UpdateInvoice(It.IsAny<InvoiceDto>()))
                .ReturnsAsync((InvoiceDto i) => Result<InvoiceDto>.Ok(i));

            var useCase = new AddInvoiceDocumentUseCase(
                invoiceRepository.Object,
                viewInvoicesUseCase.Object,
                fileManagementRepository.Object,
                editInvoiceUseCase.Object);

            var dto = new InvoiceDto { InvoiceId = 10, InvoiceNumber = "FV/2025/010" };

            var result = await useCase.AddInvoiceDocumentAsync(dto, "invoice.pdf", "application/pdf", new MemoryStream());

            Assert.True(result.Success);
            fileManagementRepository.Verify(
                f => f.AddNewFile(10, "invoice.pdf", "application/pdf", It.IsAny<Stream>()),
                Times.Once);
            editInvoiceUseCase.Verify(e => e.UpdateInvoice(It.IsAny<InvoiceDto>()), Times.Once);
        }

        [Fact]
        public async Task AddInvoiceDocumentAsync_ReturnsFailureWithMessage_WhenFileSaveFails()
        {
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var viewInvoicesUseCase = new Mock<IViewInvoicesUseCase>();
            var fileManagementRepository = new Mock<IFileManagementRepository>();
            var editInvoiceUseCase = new Mock<IEditInvoiceUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(false);

            var useCase = new AddInvoiceDocumentUseCase(
                invoiceRepository.Object,
                viewInvoicesUseCase.Object,
                fileManagementRepository.Object,
                editInvoiceUseCase.Object);

            var dto = new InvoiceDto { InvoiceId = 11, InvoiceNumber = "FV/2025/011" };

            var result = await useCase.AddInvoiceDocumentAsync(dto, "invoice.pdf", "application/pdf", Stream.Null);

            Assert.False(result.Success);
            Assert.Equal("Failed to save the invoice document file.", result.Message);
            editInvoiceUseCase.Verify(e => e.UpdateInvoice(It.IsAny<InvoiceDto>()), Times.Never);
        }

        [Fact]
        public async Task AddInvoiceDocumentAsync_SetsFilePathAndUploadDate_BeforeCallingEditInvoiceUseCase()
        {
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var viewInvoicesUseCase = new Mock<IViewInvoicesUseCase>();
            var fileManagementRepository = new Mock<IFileManagementRepository>();
            var editInvoiceUseCase = new Mock<IEditInvoiceUseCase>();

            fileManagementRepository
                .Setup(f => f.AddNewFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync(true);

            InvoiceDto? captured = null;
            editInvoiceUseCase
                .Setup(e => e.UpdateInvoice(It.IsAny<InvoiceDto>()))
                .Callback<InvoiceDto>(i => captured = i)
                .ReturnsAsync((InvoiceDto i) => Result<InvoiceDto>.Ok(i));

            var useCase = new AddInvoiceDocumentUseCase(
                invoiceRepository.Object,
                viewInvoicesUseCase.Object,
                fileManagementRepository.Object,
                editInvoiceUseCase.Object);

            var dto = new InvoiceDto { InvoiceId = 12, InvoiceNumber = "FV/2025/012" };
            var beforeCall = DateTime.Now;

            var result = await useCase.AddInvoiceDocumentAsync(dto, "receipt.pdf", "application/pdf", new MemoryStream());

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(Path.Combine("Files", "Invoices", "12", "receipt.pdf"), captured.FilePath);
            Assert.NotNull(captured.UploadDate);
            Assert.True(captured.UploadDate >= beforeCall);
        }
    }
}
