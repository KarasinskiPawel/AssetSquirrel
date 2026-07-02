using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Invoices
{
    public class AddInvoiceUseCaseTests
    {
        [Fact]
        public async Task AddInvoiceAsync_MapsDtoToEntity_AndDelegatesToUpdateInvoiceAsync()
        {
            var repository = new Mock<IInvoiceRepository>();
            Invoice? captured = null;
            repository
                .Setup(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync((Invoice i) => Result<Invoice>.Ok(i));

            var useCase = new AddInvoiceUseCase(repository.Object);
            var dto = new InvoiceDto { InvoiceId = 1, InvoiceNumber = "FV/2025/001", Description = "Laptop purchase", UserId = "user-1" };

            var result = await useCase.AddInvoiceAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.InvoiceId, captured.InvoiceId);
            Assert.Equal(dto.InvoiceNumber, captured.InvoiceNumber);
            repository.Verify(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()), Times.Once);
        }

        [Fact]
        public async Task AddInvoiceAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository
                .Setup(r => r.UpdateInvoiceAsync(It.IsAny<Invoice>()))
                .ReturnsAsync(Result<Invoice>.Fail("Database is unavailable."));

            var useCase = new AddInvoiceUseCase(repository.Object);
            var dto = new InvoiceDto { InvoiceNumber = "FV/2025/002" };

            var result = await useCase.AddInvoiceAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
