using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Invoices
{
    public class ViewInvoicesUseCase : IViewInvoicesUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public ViewInvoicesUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }
        public async Task<List<InvoiceDto>> GetInvoicesAsync(Expression<Func<Invoice, bool>> where)
        {
            return await _invoiceRepository.GetInvoicesAsync(where)
                .ContinueWith(t => t.Result.ToList(), TaskScheduler.Default);
        }

        public async Task<bool> DeleteInvoice(InvoiceDto invoice)
        {
            return await _invoiceRepository.DeleteInvoiceAsync(
                new GenericMapper<Invoice, InvoiceDto>().Map(invoice)
                );
        }
    }
}
