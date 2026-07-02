using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IAddInvoiceDocumentUseCase
    {
        Task<Result<InvoiceDto>> AddInvoiceDocumentAsync(InvoiceDto invoice, string fileName, string contentType, Stream fileStream);
    }
}