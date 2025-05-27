using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AutoMapper.QueryableExtensions;
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
        Task<bool> AddSuppilerAsync(Suppiler suppiler);
        Task<bool> DeleteSuppilerAsync(Suppiler suppiler);
        Task<IEnumerable<Suppiler>> GetSuppilersAsync(Expression<Func<Suppiler, bool>> where);
        Task<bool> UpdateSuppilerAsync(Suppiler suppiler);
    }
}
