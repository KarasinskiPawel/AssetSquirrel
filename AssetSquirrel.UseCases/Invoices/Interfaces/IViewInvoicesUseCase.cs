using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System.Linq.Expressions;

namespace AssetSquirrel.UseCases.Invoices.Interfaces
{
    public interface IViewInvoicesUseCase
    {
        Task<Result<InvoiceDto>> DeleteInvoice(InvoiceDto invoice);
        Task<List<InvoiceDto>> GetInvoicesAsync(Expression<Func<Invoice, bool>> where);
    }
}