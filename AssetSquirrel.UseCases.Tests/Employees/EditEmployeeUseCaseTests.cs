using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.Employees;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Employees
{
    public class EditEmployeeUseCaseTests
    {
        [Fact]
        public async Task EditEmployeeAsync_MapsDtoToEntity_AndDelegatesToUpdateEmployeeAsync()
        {
            var repository = new Mock<IEmployeesRepository>();
            Employee? captured = null;
            repository
                .Setup(r => r.UpdateEmployeeAsync(It.IsAny<Employee>()))
                .Callback<Employee>(e => captured = e)
                .ReturnsAsync((Employee e) => Result<Employee>.Ok(e));

            var useCase = new EditEmployeeUseCase(repository.Object);
            var dto = new EmployeeDto
            {
                EmployeeId = 3,
                FirstName = "Ewa",
                LastName = "Zielinska",
                Email = "ewa.zielinska@example.com",
                PhoneNumber = "123456789",
                IsActive = false
            };

            var result = await useCase.EditEmployeeAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.EmployeeId, captured.EmployeeId);
            Assert.Equal(dto.PhoneNumber, captured.PhoneNumber);
            Assert.Equal(dto.IsActive, captured.IsActive);
            repository.Verify(r => r.UpdateEmployeeAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task EditEmployeeAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<IEmployeesRepository>();
            repository
                .Setup(r => r.UpdateEmployeeAsync(It.IsAny<Employee>()))
                .ReturnsAsync(Result<Employee>.Fail("Employee not found."));

            var useCase = new EditEmployeeUseCase(repository.Object);
            var dto = new EmployeeDto { EmployeeId = 3, FirstName = "Ewa", LastName = "Zielinska" };

            var result = await useCase.EditEmployeeAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Employee not found.", result.Message);
        }
    }
}
