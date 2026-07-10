using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.UseCases.EquipmentReturn;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.EquipmentReturn
{
    public class ViewEquipmentReturnUseCaseTests
    {
        [Fact]
        public async Task GetEquipmentReturnsAsync_ReturnsMappedAndFlattenedDtos()
        {
            var manufacturer = new Manufacturer { ManufacturerId = 1, Name = "Dell" };
            var hardwareType = new CoreBusiness.HardwareType { HardwareTypeId = 1, Name = "Laptop" };
            var equipment = new CoreBusiness.Equipment { EquipmentId = 5, ModelName = "Latitude", SerialNumber = "SN-100", InventoryNumber = "49100000030", Manufacturer = manufacturer, HardwareType = hardwareType };
            var storageLocation = new Location { LocationId = 3, City = "Łódź", Street = "Magazyn IT", EquipmentStorage = true };
            var employee = new Employee { EmployeeId = 7, FirstName = "Jan", LastName = "Kowalski" };

            var returns = new List<AssetSquirrel.CoreBusiness.EquipmentReturn>
            {
                new()
                {
                    EquipmentReturnId = 1,
                    ReturnDocumentNumber = "2026/07/0001",
                    EmployeeId = 7,
                    Employee = employee,
                    ReturnDate = new DateTime(2026, 7, 7),
                    Comment = "Returned on leave",
                    StorageLocationId = 3,
                    StorageLocation = storageLocation,
                    PreparedByUserId = "user-1",
                    EquipmentAssignments = new List<AssetSquirrel.CoreBusiness.EquipmentAssignment>
                    {
                        new()
                        {
                            EquipmentAssignmentId = 100,
                            EquipmentId = 5,
                            Equipment = equipment,
                            DateOfHandover = new DateTime(2026, 1, 1),
                            DateOfReturn = new DateTime(2026, 7, 7)
                        }
                    }
                }
            };

            var repository = new Mock<IEquipmentReturnRepository>();
            repository
                .Setup(r => r.GetEquipmentReturnsAsync(It.IsAny<Expression<Func<AssetSquirrel.CoreBusiness.EquipmentReturn, bool>>>()))
                .ReturnsAsync(returns);

            var useCase = new ViewEquipmentReturnUseCase(repository.Object);

            var result = await useCase.GetEquipmentReturnsAsync(r => true);

            Assert.Single(result);
            var dto = result[0];
            Assert.Equal("2026/07/0001", dto.ReturnDocumentNumber);
            Assert.Equal("Łódź Magazyn IT", dto.StorageLocationName);
            Assert.Single(dto.Items);
            Assert.Equal("Dell", dto.Items[0].ManufacturerName);
            Assert.Equal("Laptop", dto.Items[0].HardwareTypeName);
            Assert.Equal("Latitude", dto.Items[0].ModelName);
            Assert.Equal("SN-100", dto.Items[0].SerialNumber);
            Assert.Equal("49100000030", dto.Items[0].InventoryNumber);
        }
    }
}
