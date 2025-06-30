using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Invoices.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.InMemory.Files;
using AssetsSquirrel.Plugins.InMemory.Files.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.Invoices
{
    public class AddInvoiceDocumentUseCase : IAddInvoiceDocumentUseCase
    {
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IViewInvoicesUseCase viewInvoicesUseCase;
        private readonly IFileManagementRepository fileManagementRepository;

        public AddInvoiceDocumentUseCase(
            IInvoiceRepository invoiceRepository,
            IViewInvoicesUseCase viewInvoicesUseCase,
            IFileManagementRepository fileManagementRepository
            )
        {
            this.invoiceRepository = invoiceRepository;
            this.viewInvoicesUseCase = viewInvoicesUseCase;
            this.fileManagementRepository = fileManagementRepository;
        }

        public async Task<bool> AddInvoiceDocumentAsync(InvoiceDto invoice, string fileName, string contentType, Stream fileStream)
        {
            if(fileManagementRepository.AddNewFile(invoice.InvoiceId, fileName, contentType, fileStream))
            {
                invoice.FilePath = System.IO.Path.Combine("Files", "Invoices", invoice.InvoiceId.ToString(), fileName);
                invoice.UploadDate = DateTime.Now;

                return await viewInvoicesUseCase.UpdateInvoice(invoice);
            }

            return false;
        }
    }
}
