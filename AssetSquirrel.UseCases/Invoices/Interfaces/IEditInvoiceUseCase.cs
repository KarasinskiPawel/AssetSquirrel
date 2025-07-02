using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IEditInvoiceUseCase
    {
        Task<bool> UpdateInvoice(InvoiceDto invoice);
    }
}