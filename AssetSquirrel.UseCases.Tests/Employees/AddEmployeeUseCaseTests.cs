using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Employees
{
    public class AddEmployeeUseCaseTests
    {
        [Fact]
        public async Task AddEmployeeAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<IEmployeesRepository>();
            Employee? captured = null;
            repository
                .Setup(r => r.AddEmployeeAsync(It.IsAny<Employee>()))
                .Callback<Employee>(e => captured = e)
                .ReturnsAsync((Employee e) => Result<Employee>.Ok(e));

            var useCase = new AddEmployeeUseCase(repository.Object);
            var dto = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                IsActive = true
            };

            var result = await useCase.AddEmployeeAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.EmployeeId, captured.EmployeeId);
            Assert.Equal(dto.FirstName, captured.FirstName);
            Assert.Equal(dto.LastName, captured.LastName);
            Assert.Equal(dto.Email, captured.Email);
            repository.Verify(r => r.AddEmployeeAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task AddEmployeeAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IEmployeesRepository>();
            repository
                .Setup(r => r.AddEmployeeAsync(It.IsAny<Employee>()))
                .ReturnsAsync(Result<Employee>.Fail("Database is unavailable."));

            var useCase = new AddEmployeeUseCase(repository.Object);
            var dto = new EmployeeDto { FirstName = "Jan", LastName = "Kowalski" };

            var result = await useCase.AddEmployeeAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
