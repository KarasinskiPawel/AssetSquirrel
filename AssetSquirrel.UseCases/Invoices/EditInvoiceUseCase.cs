using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices.Interfaces;
using AssetSquirrel.UseCases.Mapper;
using AssetSquirrel.UseCases.PluginInterfaces;
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

        public async Task<bool> UpdateInvoice(InvoiceDto invoice)
        {
            return await invoiceRepository.UpdateInvoiceAsync(
                new GenericMapper<Invoice, InvoiceDto>().Map(invoice)
                );
        }
    }
}
