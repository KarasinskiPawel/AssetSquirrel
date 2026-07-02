using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentHandover;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentHandover
{
    public class AddEquipmentHandoverUseCaseTests
    {
        [Fact]
        public async Task GetEquipmentAsync_ReturnsRepositoryResultDirectly()
        {
            var equipment = new List<EquipmentDto>
            {
                new() { EquipmentId = 1, ModelName = "Laptop Dell", SerialNumber = "SN-001", IsActive = true },
                new() { EquipmentId = 2, ModelName = "Monitor LG", SerialNumber = "SN-002", IsActive = false }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            equipmentRepository
                .Setup(r => r.GetEquipmentAsync(It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>()))
                .ReturnsAsync(equipment);

            var locationRepository = new Mock<ILocationRepository>();
            var employeesRepository = new Mock<IEmployeesRepository>();

            var useCase = new AddEquipmentHandoverUseCase(equipmentRepository.Object, locationRepository.Object, employeesRepository.Object);

            var result = await useCase.GetEquipmentAsync(e => e.IsActive);

            Assert.Same(equipment, result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.EquipmentId == 1 && dto.ModelName == "Laptop Dell" && dto.SerialNumber == "SN-001");
            Assert.Contains(result, dto => dto.EquipmentId == 2 && dto.ModelName == "Monitor LG");
        }

        [Fact]
        public async Task GetLocationsAsync_ReturnsMappedDtos()
        {
            var locations = new List<Location>
            {
                new() { LocationId = 1, Code = "WAW", City = "Warszawa", Street = "Marszałkowska", IsActive = true },
                new() { LocationId = 2, Code = "KRK", City = "Kraków", Street = "Floriańska", IsActive = false }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            locationRepository
                .Setup(r => r.GetLocationsAsync(It.IsAny<Expression<Func<Location, bool>>>()))
                .ReturnsAsync(locations);
            var employeesRepository = new Mock<IEmployeesRepository>();

            var useCase = new AddEquipmentHandoverUseCase(equipmentRepository.Object, locationRepository.Object, employeesRepository.Object);

            var result = await useCase.GetLocationsAsync(l => l.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.LocationId == 1 && dto.City == "Warszawa" && dto.Code == "WAW");
            Assert.Contains(result, dto => dto.LocationId == 2 && dto.City == "Kraków" && dto.Code == "KRK");
        }

        [Fact]
        public async Task GetEmployeesAsync_ReturnsMappedDtos()
        {
            var employees = new List<Employee>
            {
                new() { EmployeeId = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@example.com", IsActive = true },
                new() { EmployeeId = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@example.com", IsActive = false }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var employeesRepository = new Mock<IEmployeesRepository>();
            employeesRepository
                .Setup(r => r.GetEmployeesAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(employees);

            var useCase = new AddEquipmentHandoverUseCase(equipmentRepository.Object, locationRepository.Object, employeesRepository.Object);

            var result = await useCase.GetEmployeesAsync(e => e.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.EmployeeId == 1 && dto.FirstName == "Jan" && dto.LastName == "Kowalski");
            Assert.Contains(result, dto => dto.EmployeeId == 2 && dto.FirstName == "Anna" && dto.LastName == "Nowak");
        }
    }
}
