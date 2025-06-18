using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public InvoiceRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }
        public async Task<bool> DeleteInvoiceAsync(Invoice invoice)
        {
            try
            {
                if (invoice is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();
                    dbContext.Invoices.Remove(invoice);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "InvoiceRepository", "DeleteInvoiceAsync", e);
                return false;
            }
        }

        public async Task<IEnumerable<InvoiceDto>> GetInvoicesAsync(Expression<Func<Invoice, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            var output = await dbContext.Invoices
                .Where(where)
                .Include(i => i.User)
                .Select(a => new InvoiceDto {
                    InvoiceId = a.InvoiceId,
                    InvoiceNumber = a.InvoiceNumber,
                    Description = a.Description,
                    FilePath = a.FilePath,
                    InvoiceDate = a.InvoiceDate,
                    UploadDate = a.UploadDate,
                    UserId = a.UserId,
                    UserName = a.User != null ? a.User.UserName : null
                })
                .ToListAsync() ?? Enumerable.Empty<InvoiceDto>();

            return output;
        }

        public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
        {
            try
            {
                if(invoice is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();
                    dbContext.Invoices.Update(invoice);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "InvoiceRepository", "UpdateInvoiceAsync", e);
                return false;
            }
        }
    }
}
