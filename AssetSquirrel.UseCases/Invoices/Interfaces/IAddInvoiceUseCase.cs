using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IAddInvoiceUseCase
    {
        Task<bool> AddInvoiceAsync(InvoiceDto invoice);
    }
}