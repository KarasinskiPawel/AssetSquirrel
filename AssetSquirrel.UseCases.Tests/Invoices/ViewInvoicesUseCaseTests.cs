using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Invoices
{
    public class ViewInvoicesUseCaseTests
    {
        [Fact]
        public async Task GetInvoicesAsync_ReturnsRepositoryDtos()
        {
            var invoices = new List<InvoiceDto>
            {
                new() { InvoiceId = 1, InvoiceNumber = "FV/2025/001", Description = "Laptop purchase" },
                new() { InvoiceId = 2, InvoiceNumber = "FV/2025/002", Description = "Office chair" }
            };

            var repository = new Mock<IInvoiceRepository>();
            repository
                .Setup(r => r.GetInvoicesAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync(invoices);

            var useCase = new ViewInvoicesUseCase(repository.Object);

            var result = await useCase.GetInvoicesAsync(i => i.InvoiceId > 0);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.InvoiceId == 1 && dto.InvoiceNumber == "FV/2025/001");
            Assert.Contains(result, dto => dto.InvoiceId == 2 && dto.Description == "Office chair");
        }

        [Fact]
        public async Task DeleteInvoice_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<IInvoiceRepository>();
            Invoice? captured = null;
            repository
                .Setup(r => r.DeleteInvoiceAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => captured = i)
                .ReturnsAsync((Invoice i) => Result<Invoice>.Ok(i));

            var useCase = new ViewInvoicesUseCase(repository.Object);
            var dto = new InvoiceDto { InvoiceId = 5, InvoiceNumber = "FV/2025/005" };

            var result = await useCase.DeleteInvoice(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(5, captured.InvoiceId);
            repository.Verify(r => r.DeleteInvoiceAsync(It.IsAny<Invoice>()), Times.Once);
        }

        [Fact]
        public async Task DeleteInvoice_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IInvoiceRepository>();
            repository
                .Setup(r => r.DeleteInvoiceAsync(It.IsAny<Invoice>()))
                .ReturnsAsync(Result<Invoice>.Fail("Invoice cannot be deleted."));

            var useCase = new ViewInvoicesUseCase(repository.Object);
            var dto = new InvoiceDto { InvoiceId = 5, InvoiceNumber = "FV/2025/005" };

            var result = await useCase.DeleteInvoice(dto);

            Assert.False(result.Success);
            Assert.Equal("Invoice cannot be deleted.", result.Message);
        }
    }
}
