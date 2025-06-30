using AssetSquirrel.UseCases.Invoices;
using AssetSquirrel.UseCases.Invoices.Interfaces;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories;

namespace AssetSquirrelAuthorize.WebApp.Extensions
{
    public static class InvoicesUseCaseExtensions
    {
        public static IServiceCollection AddExtension(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            services.AddScoped<IViewInvoicesUseCase, ViewInvoicesUseCase>();
            services.AddScoped<IAddInvoiceUseCase, AddInvoiceUseCase>();
            services.AddScoped<IAddInvoiceDocumentUseCase, AddInvoiceDocumentUseCase>();

            return services;
        }
    }
}
