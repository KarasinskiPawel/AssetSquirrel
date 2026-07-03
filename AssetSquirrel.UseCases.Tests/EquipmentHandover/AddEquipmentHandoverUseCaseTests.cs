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
        private static AddEquipmentHandoverUseCase CreateUseCase(
            Mock<IEquipmentRepository> equipmentRepository,
            Mock<ILocationRepository> locationRepository,
            Mock<IEmployeesRepository> employeesRepository,
            Mock<IEquipmentAssignmentRepository> equipmentAssignmentRepository,
            Mock<IEquipmentHandoverRepository> equipmentHandoverRepository)
        {
            return new AddEquipmentHandoverUseCase(
                equipmentRepository.Object,
                locationRepository.Object,
                employeesRepository.Object,
                equipmentAssignmentRepository.Object,
                equipmentHandoverRepository.Object);
        }

        [Fact]
        public async Task GetEquipmentAsync_ExcludesAlreadyAssignedEquipment()
        {
            var equipment = new List<EquipmentDto>
            {
                new() { EquipmentId = 1, ModelName = "Laptop Dell", SerialNumber = "SN-001", IsActive = true },
                new() { EquipmentId = 2, ModelName = "Monitor LG", SerialNumber = "SN-002", IsActive = true }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            equipmentRepository
                .Setup(r => r.GetEquipmentAsync(It.IsAny<Expression<Func<CoreBusiness.Equipment, bool>>>()))
                .ReturnsAsync(equipment);

            var locationRepository = new Mock<ILocationRepository>();
            var employeesRepository = new Mock<IEmployeesRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            equipmentAssignmentRepository
                .Setup(r => r.GetAssignedEquipmentIdsAsync())
                .ReturnsAsync(new List<int> { 2 });
            var equipmentHandoverRepository = new Mock<IEquipmentHandoverRepository>();

            var useCase = CreateUseCase(equipmentRepository, locationRepository, employeesRepository, equipmentAssignmentRepository, equipmentHandoverRepository);

            var result = await useCase.GetEquipmentAsync(e => e.IsActive);

            Assert.Single(result);
            Assert.Contains(result, dto => dto.EquipmentId == 1 && dto.ModelName == "Laptop Dell" && dto.SerialNumber == "SN-001");
            Assert.DoesNotContain(result, dto => dto.EquipmentId == 2);
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
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentHandoverRepository = new Mock<IEquipmentHandoverRepository>();

            var useCase = CreateUseCase(equipmentRepository, locationRepository, employeesRepository, equipmentAssignmentRepository, equipmentHandoverRepository);

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
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentHandoverRepository = new Mock<IEquipmentHandoverRepository>();

            var useCase = CreateUseCase(equipmentRepository, locationRepository, employeesRepository, equipmentAssignmentRepository, equipmentHandoverRepository);

            var result = await useCase.GetEmployeesAsync(e => e.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.EmployeeId == 1 && dto.FirstName == "Jan" && dto.LastName == "Kowalski");
            Assert.Contains(result, dto => dto.EmployeeId == 2 && dto.FirstName == "Anna" && dto.LastName == "Nowak");
        }

        [Fact]
        public async Task SaveHandoverAsync_BuildsDetailsFromEquipmentIds_AndReturnsMappedDto()
        {
            var equipmentRepository = new Mock<IEquipmentRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var employeesRepository = new Mock<IEmployeesRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentHandoverRepository = new Mock<IEquipmentHandoverRepository>();

            CoreBusiness.EquipmentHandover? capturedEntity = null;
            equipmentHandoverRepository
                .Setup(r => r.PostEquipmentHandoverAsync(It.IsAny<CoreBusiness.EquipmentHandover>(), "user-1"))
                .Callback<CoreBusiness.EquipmentHandover, string>((entity, _) =>
                {
                    capturedEntity = entity;
                    entity.EquipmentHandoverId = 42;
                    entity.HandoverDocumentNumber = "2026/07/0001";
                })
                .ReturnsAsync((CoreBusiness.EquipmentHandover entity, string _) => Result<CoreBusiness.EquipmentHandover>.Ok(entity));

            var useCase = CreateUseCase(equipmentRepository, locationRepository, employeesRepository, equipmentAssignmentRepository, equipmentHandoverRepository);

            var handover = new EquipmentHandoverDto { ToEmployeeId = 5, HandoverDate = new DateTime(2026, 7, 3) };

            var result = await useCase.SaveHandoverAsync(handover, new List<int> { 10, 11 }, "user-1");

            Assert.True(result.Success);
            Assert.Equal(42, result.Data!.EquipmentHandoverId);
            Assert.Equal("2026/07/0001", result.Data.HandoverDocumentNumber);
            Assert.NotNull(capturedEntity);
            Assert.Equal(2, capturedEntity!.EquipmentHandoverDetails.Count);
            Assert.Contains(capturedEntity.EquipmentHandoverDetails, d => d.EquipmentId == 10);
            Assert.Contains(capturedEntity.EquipmentHandoverDetails, d => d.EquipmentId == 11);
        }

        [Fact]
        public async Task SaveHandoverAsync_PropagatesFailureFromRepository()
        {
            var equipmentRepository = new Mock<IEquipmentRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var employeesRepository = new Mock<IEmployeesRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentHandoverRepository = new Mock<IEquipmentHandoverRepository>();
            equipmentHandoverRepository
                .Setup(r => r.PostEquipmentHandoverAsync(It.IsAny<CoreBusiness.EquipmentHandover>(), It.IsAny<string>()))
                .ReturnsAsync(Result<CoreBusiness.EquipmentHandover>.Fail("One or more items have already been handed over to someone else. Please refresh and try again."));

            var useCase = CreateUseCase(equipmentRepository, locationRepository, employeesRepository, equipmentAssignmentRepository, equipmentHandoverRepository);

            var result = await useCase.SaveHandoverAsync(new EquipmentHandoverDto(), new List<int> { 10 }, "user-1");

            Assert.False(result.Success);
            Assert.Equal("One or more items have already been handed over to someone else. Please refresh and try again.", result.Message);
        }
    }
}
