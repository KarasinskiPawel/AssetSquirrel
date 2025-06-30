using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IAddInvoiceDocumentUseCase
    {
        Task<bool> AddInvoiceDocumentAsync(InvoiceDto invoice, string fileName, string contentType, Stream fileStream);
    }
}