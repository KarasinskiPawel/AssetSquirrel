using AssetSquirrel.CoreBusiness;
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
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly IDbContextFactory<AssetsSquirrelContext> dbContextFactory;
        private readonly IErrorsRepository errorsRepository;

        public EmployeesRepository(IDbContextFactory<AssetsSquirrelContext> dbContextFactory, IErrorsRepository errorsRepository)
        {
            this.dbContextFactory = dbContextFactory;
            this.errorsRepository = errorsRepository;
        }
        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            try
            {
                if (employee is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();
                    await dbContext.Employees.AddAsync(employee);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EmployeesRepository", "AddEmployeeAsync", ex);
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(Employee employee)
        {
            try
            {
                if (employee is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Employees.Remove(employee);
                    await dbContext.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EmployeesRepository", "DeleteEmployeeAsync", ex);
                return false;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> where)
        {
            var dbContext = dbContextFactory.CreateDbContext();

            return await dbContext.Employees
                .Where(where)
                .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                if(employee is not null)
                {
                    var dbContext = dbContextFactory.CreateDbContext();

                    dbContext.Employees.Update(employee);
                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                await errorsRepository.AddErrorAsync("AssetsSquirrel.Plugins.EFCoreSqlServer.Repositories", "EmployeesRepository", "UpdateEmployeeAsync", ex);
                return false;
            }
        }
    }
}
