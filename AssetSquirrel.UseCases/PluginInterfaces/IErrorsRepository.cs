using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetSquirrel.CoreBusiness;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IErrorsRepository
    {
        Task<Result<bool>> AddErrorAsync(string serviceName, string className, string methodName, Exception exception);
    }
}
