using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IInvoiceRepository
    {
        Task<List<InvoiceDto>> GetInvoicesAsync(Expression<Func<Invoice, bool>> where);
        Task<Result<Invoice>> UpdateInvoiceAsync(Invoice invoice);
        Task<Result<Invoice>> DeleteInvoiceAsync(Invoice invoiceId);
    }
}
