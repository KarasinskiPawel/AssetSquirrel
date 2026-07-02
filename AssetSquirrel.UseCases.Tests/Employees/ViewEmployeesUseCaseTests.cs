using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Employees
{
    public class ViewEmployeesUseCaseTests
    {
        [Fact]
        public async Task GetEmployeesAsync_ReturnsMappedDtos()
        {
            var employees = new List<Employee>
            {
                new() { EmployeeId = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@example.com", IsActive = true },
                new() { EmployeeId = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@example.com", IsActive = false }
            };

            var repository = new Mock<IEmployeesRepository>();
            repository
                .Setup(r => r.GetEmployeesAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(employees);

            var useCase = new ViewEmployeesUseCase(repository.Object);

            var result = await useCase.GetEmployeesAsync(e => e.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.EmployeeId == 1 && dto.FirstName == "Jan" && dto.Email == "jan.kowalski@example.com");
            Assert.Contains(result, dto => dto.EmployeeId == 2 && dto.LastName == "Nowak");
        }

        [Fact]
        public async Task DeleteEmployeeAsync_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<IEmployeesRepository>();
            Employee? captured = null;
            repository
                .Setup(r => r.DeleteEmployeeAsync(It.IsAny<Employee>()))
                .Callback<Employee>(e => captured = e)
                .ReturnsAsync((Employee e) => Result<Employee>.Ok(e));

            var useCase = new ViewEmployeesUseCase(repository.Object);
            var dto = new EmployeeDto { EmployeeId = 5, FirstName = "Piotr", LastName = "Wisniewski" };

            var result = await useCase.DeleteEmployeeAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(5, captured.EmployeeId);
            repository.Verify(r => r.DeleteEmployeeAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployee_DelegatesToRepositoryUpdateEmployeeAsync()
        {
            var repository = new Mock<IEmployeesRepository>();
            repository
                .Setup(r => r.UpdateEmployeeAsync(It.IsAny<Employee>()))
                .ReturnsAsync((Employee e) => Result<Employee>.Ok(e));

            var useCase = new ViewEmployeesUseCase(repository.Object);
            var dto = new EmployeeDto { EmployeeId = 7, FirstName = "Ola", LastName = "Kwiatkowska" };

            var result = await useCase.UpdateEmployee(dto);

            Assert.True(result.Success);
            repository.Verify(r => r.UpdateEmployeeAsync(It.Is<Employee>(e => e.EmployeeId == 7)), Times.Once);
        }
    }
}
