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
    public interface ISuppilersRepository
    {
        Task<Result<Suppiler>> AddSuppilerAsync(Suppiler suppiler);
        Task<Result<Suppiler>> DeleteSuppilerAsync(Suppiler suppiler);
        Task<IEnumerable<Suppiler>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where);
        Task<Result<Suppiler>> UpdateSuppilerAsync(Suppiler suppiler);
    }
}
