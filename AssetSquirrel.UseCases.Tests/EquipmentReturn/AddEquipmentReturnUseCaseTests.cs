using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentReturn;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentReturn
{
    public class AddEquipmentReturnUseCaseTests
    {
        private static AddEquipmentReturnUseCase CreateUseCase(
            Mock<IEmployeesRepository> employeesRepository,
            Mock<ILocationRepository> locationRepository,
            Mock<IEquipmentAssignmentRepository> equipmentAssignmentRepository,
            Mock<IEquipmentReturnRepository> equipmentReturnRepository)
        {
            return new AddEquipmentReturnUseCase(
                employeesRepository.Object,
                locationRepository.Object,
                equipmentAssignmentRepository.Object,
                equipmentReturnRepository.Object);
        }

        [Fact]
        public async Task GetEmployeesAsync_ReturnsMappedDtos()
        {
            var employees = new List<Employee>
            {
                new() { EmployeeId = 1, FirstName = "Jan", LastName = "Kowalski", IsActive = true }
            };

            var employeesRepository = new Mock<IEmployeesRepository>();
            employeesRepository
                .Setup(r => r.GetEmployeesAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(employees);
            var locationRepository = new Mock<ILocationRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentReturnRepository = new Mock<IEquipmentReturnRepository>();

            var useCase = CreateUseCase(employeesRepository, locationRepository, equipmentAssignmentRepository, equipmentReturnRepository);

            var result = await useCase.GetEmployeesAsync(e => e.IsActive);

            Assert.Single(result);
            Assert.Contains(result, dto => dto.EmployeeId == 1 && dto.FirstName == "Jan");
        }

        [Fact]
        public async Task GetLocationsAsync_ReturnsMappedDtos()
        {
            var locations = new List<Location>
            {
                new() { LocationId = 3, City = "Łódź", Street = "Magazyn IT", EquipmentStorage = true, IsActive = true }
            };

            var employeesRepository = new Mock<IEmployeesRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            locationRepository
                .Setup(r => r.GetLocationsAsync(It.IsAny<Expression<Func<Location, bool>>>()))
                .ReturnsAsync(locations);
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentReturnRepository = new Mock<IEquipmentReturnRepository>();

            var useCase = CreateUseCase(employeesRepository, locationRepository, equipmentAssignmentRepository, equipmentReturnRepository);

            var result = await useCase.GetLocationsAsync(l => l.EquipmentStorage && l.IsActive);

            Assert.Single(result);
            Assert.Contains(result, dto => dto.LocationId == 3 && dto.EquipmentStorage);
        }

        [Fact]
        public async Task GetOpenAssignmentsAsync_ReturnsEmpty_WithoutQueryingRepository_WhenNoRecipientSelected()
        {
            var employeesRepository = new Mock<IEmployeesRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentReturnRepository = new Mock<IEquipmentReturnRepository>();

            var useCase = CreateUseCase(employeesRepository, locationRepository, equipmentAssignmentRepository, equipmentReturnRepository);

            var result = await useCase.GetOpenAssignmentsAsync(null, null);

            Assert.Empty(result);
            equipmentAssignmentRepository.Verify(r => r.GetOpenAssignmentsAsync(It.IsAny<Expression<Func<EquipmentAssignment, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task GetOpenAssignmentsAsync_FiltersByEmployeeOrLocation_AndReturnsFlattenedDtos()
        {
            var manufacturer = new Manufacturer { ManufacturerId = 1, Name = "HP" };
            var hardwareType = new CoreBusiness.HardwareType { HardwareTypeId = 2, Name = "Monitor" };

            var matchingByEmployee = new EquipmentAssignment
            {
                EquipmentAssignmentId = 1,
                EquipmentId = 10,
                EmployeeId = 7,
                LocationId = null,
                DateOfReturn = null,
                Equipment = new CoreBusiness.Equipment { EquipmentId = 10, ModelName = "E24", SerialNumber = "SN-1", Manufacturer = manufacturer, HardwareType = hardwareType }
            };
            var matchingByLocation = new EquipmentAssignment
            {
                EquipmentAssignmentId = 2,
                EquipmentId = 11,
                EmployeeId = null,
                LocationId = 3,
                DateOfReturn = null,
                Equipment = new CoreBusiness.Equipment { EquipmentId = 11, ModelName = "E25", SerialNumber = "SN-2", Manufacturer = manufacturer, HardwareType = hardwareType }
            };
            var nonMatching = new EquipmentAssignment
            {
                EquipmentAssignmentId = 3,
                EquipmentId = 12,
                EmployeeId = 999,
                LocationId = 999,
                DateOfReturn = null,
                Equipment = new CoreBusiness.Equipment { EquipmentId = 12, ModelName = "E26", SerialNumber = "SN-3" }
            };
            var alreadyReturned = new EquipmentAssignment
            {
                EquipmentAssignmentId = 4,
                EquipmentId = 13,
                EmployeeId = 7,
                DateOfReturn = new DateTime(2026, 1, 1),
                Equipment = new CoreBusiness.Equipment { EquipmentId = 13, ModelName = "E27", SerialNumber = "SN-4" }
            };

            var allAssignments = new List<EquipmentAssignment> { matchingByEmployee, matchingByLocation, nonMatching, alreadyReturned };

            Expression<Func<EquipmentAssignment, bool>>? capturedPredicate = null;

            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            equipmentAssignmentRepository
                .Setup(r => r.GetOpenAssignmentsAsync(It.IsAny<Expression<Func<EquipmentAssignment, bool>>>()))
                .Callback<Expression<Func<EquipmentAssignment, bool>>>(expr => capturedPredicate = expr)
                .ReturnsAsync(() => allAssignments.Where(capturedPredicate!.Compile()).ToList());

            var employeesRepository = new Mock<IEmployeesRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var equipmentReturnRepository = new Mock<IEquipmentReturnRepository>();

            var useCase = CreateUseCase(employeesRepository, locationRepository, equipmentAssignmentRepository, equipmentReturnRepository);

            var result = await useCase.GetOpenAssignmentsAsync(7, 3);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.EquipmentAssignmentId == 1 && dto.ManufacturerName == "HP" && dto.HardwareTypeName == "Monitor" && dto.ModelName == "E24" && dto.SerialNumber == "SN-1");
            Assert.Contains(result, dto => dto.EquipmentAssignmentId == 2);
            Assert.DoesNotContain(result, dto => dto.EquipmentAssignmentId == 3);
            Assert.DoesNotContain(result, dto => dto.EquipmentAssignmentId == 4);
        }

        [Fact]
        public async Task SaveReturnAsync_ReturnsMappedDto_OnSuccess()
        {
            var employeesRepository = new Mock<IEmployeesRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentReturnRepository = new Mock<IEquipmentReturnRepository>();

            AssetSquirrel.CoreBusiness.EquipmentReturn? capturedEntity = null;
            equipmentReturnRepository
                .Setup(r => r.PostEquipmentReturnAsync(It.IsAny<AssetSquirrel.CoreBusiness.EquipmentReturn>(), It.IsAny<List<int>>(), "user-1"))
                .Callback<AssetSquirrel.CoreBusiness.EquipmentReturn, List<int>, string>((entity, _, _) =>
                {
                    capturedEntity = entity;
                    entity.EquipmentReturnId = 9;
                    entity.ReturnDocumentNumber = "2026/07/0001";
                })
                .ReturnsAsync((AssetSquirrel.CoreBusiness.EquipmentReturn entity, List<int> _, string _) => Result<AssetSquirrel.CoreBusiness.EquipmentReturn>.Ok(entity));

            var useCase = CreateUseCase(employeesRepository, locationRepository, equipmentAssignmentRepository, equipmentReturnRepository);

            var dto = new EquipmentReturnDto { EmployeeId = 7, StorageLocationId = 3, ReturnDate = new DateTime(2026, 7, 7) };

            var result = await useCase.SaveReturnAsync(dto, new List<int> { 1, 2 }, "user-1");

            Assert.True(result.Success);
            Assert.Equal(9, result.Data!.EquipmentReturnId);
            Assert.Equal("2026/07/0001", result.Data.ReturnDocumentNumber);
            Assert.NotNull(capturedEntity);
            Assert.Equal(3, capturedEntity!.StorageLocationId);
        }

        [Fact]
        public async Task SaveReturnAsync_PropagatesFailureFromRepository()
        {
            var employeesRepository = new Mock<IEmployeesRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            var equipmentAssignmentRepository = new Mock<IEquipmentAssignmentRepository>();
            var equipmentReturnRepository = new Mock<IEquipmentReturnRepository>();
            equipmentReturnRepository
                .Setup(r => r.PostEquipmentReturnAsync(It.IsAny<AssetSquirrel.CoreBusiness.EquipmentReturn>(), It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(Result<AssetSquirrel.CoreBusiness.EquipmentReturn>.Fail("One or more selected items have already been returned. Please refresh and try again."));

            var useCase = CreateUseCase(employeesRepository, locationRepository, equipmentAssignmentRepository, equipmentReturnRepository);

            var result = await useCase.SaveReturnAsync(new EquipmentReturnDto(), new List<int> { 1 }, "user-1");

            Assert.False(result.Success);
            Assert.Equal("One or more selected items have already been returned. Please refresh and try again.", result.Message);
        }
    }
}
