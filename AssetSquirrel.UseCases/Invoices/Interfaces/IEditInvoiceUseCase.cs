using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IEditInvoiceUseCase
    {
        Task<Result<InvoiceDto>> UpdateInvoice(InvoiceDto invoice);
    }
}