using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Invoices
{
    public class EditInvoiceUseCase : IEditInvoiceUseCase
    {
        private readonly IInvoiceRepository invoiceRepository;

        public EditInvoiceUseCase(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }

        public async Task<Result<InvoiceDto>> UpdateInvoice(InvoiceDto invoice)
        {
            var result = await invoiceRepository.UpdateInvoiceAsync(invoice.Adapt<Invoice>());

            return result.Select(i => i.Adapt<InvoiceDto>());
        }
    }
}
