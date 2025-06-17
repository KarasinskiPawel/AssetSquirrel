using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories
{
    public class ErrorsRepository : IErrorsRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;

        public ErrorsRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        public async Task<bool> AddErrorAsync(string serviceName, string className, string methodName, Exception exception)
        {
            try
            {
                var dbContext = dbContextFactory.CreateDbContext();

                Error error = new Error();

                error.Date = DateTime.Now;
                error.Service = serviceName;
                error.Class = className;
                error.Method = methodName;
                error.InnerException = exception.InnerException.ToString();
                error.Message = exception.Message;
                error.Source = exception.Source;
                error.StackTrace = exception.StackTrace;
                error.TargetSite = exception.TargetSite.ToString();
                error.UserLogin = "";

                dbContext.Add(error);
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
