using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.UseCases.PluginInterfaces
{
    public interface IErrorsRepository
    {
        Task<bool> AddErrorAsync(string serviceName, string className, string methodName, Exception exception);
    }
}
