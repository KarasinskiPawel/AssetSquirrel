using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IAddInvoiceUseCase
    {
        Task<Result<InvoiceDto>> AddInvoiceAsync(InvoiceDto invoice);
    }
}