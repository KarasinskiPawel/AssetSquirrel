using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Invoices
{
    public class EditInvoiceUseCaseTests
    {
        [Fact]
        public async Task UpdateInvoice_MapsDtoToEntity_AndDelegatesToUpdateInvoiceAsync()
        {
            var repository = new Mock<IInvoiceRepository>();
            Invoice? captured = null;
            repository
                .Setup(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync((Invoice i) => Result<Invoice>.Ok(i));

            var useCase = new EditInvoiceUseCase(repository.Object);
            var dto = new InvoiceDto
            {
                InvoiceId = 3,
                InvoiceNumber = "FV/2025/003",
                Description = "Office chair",
                InvoiceDate = new DateTime(2025, 3, 15),
                UserId = "user-2"
            };

            var result = await useCase.UpdateInvoice(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.InvoiceId, captured.InvoiceId);
            Assert.Equal(dto.InvoiceNumber, captured.InvoiceNumber);
            Assert.Equal(dto.InvoiceDate, captured.InvoiceDate);
            repository.Verify(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()), Times.Once);
        }

        [Fact]
        public async Task UpdateInvoice_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository
                .Setup(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()))
                .ReturnsAsync(Result<Invoice>.Fail("Invoice not found."));

            var useCase = new EditInvoiceUseCase(repository.Object);
            var dto = new InvoiceDto { InvoiceId = 3, InvoiceNumber = "FV/2025/003" };

            var result = await useCase.UpdateInvoice(dto);

            Assert.False(result.Success);
            Assert.Equal("Invoice not found.", result.Message);
        }
    }
}
