using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.EquipmentUseCase;
using AssetSquirrel.UseCases.PluginInterfaces;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Equipment
{
    public class AddEquipmentUseCaseTests
    {
        [Fact]
        public async Task AddEquipmentAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var equipmentRepository = new Mock<IEquipmentRepository>();
            CoreBusiness.Equipment? captured = null;
            equipmentRepository
                .Setup(r => r.AddEquipmentAsync(It.IsAny<CoreBusiness.Equipment>()))
                .Callback<CoreBusiness.Equipment>(e => captured = e)
                .ReturnsAsync((CoreBusiness.Equipment e) => Result<CoreBusiness.Equipment>.Ok(e));

            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            var suppilersRepository = new Mock<ISuppilersRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            var locationRepository = new Mock<ILocationRepository>();

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var dto = new EquipmentDto
            {
                EquipmentId = 1,
                SuppilerId = 2,
                ManufacturerId = 3,
                HardwareTypeId = 4,
                ModelName = "ThinkPad T14",
                SerialNumber = "SN-12345"
            };

            var result = await useCase.AddEquipmentAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.EquipmentId, captured.EquipmentId);
            Assert.Equal(dto.ModelName, captured.ModelName);
            Assert.Equal(dto.SerialNumber, captured.SerialNumber);
            equipmentRepository.Verify(r => r.AddEquipmentAsync(It.IsAny<CoreBusiness.Equipment>()), Times.Once);
        }

        [Fact]
        public async Task AddEquipmentAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var equipmentRepository = new Mock<IEquipmentRepository>();
            equipmentRepository
                .Setup(r => r.AddEquipmentAsync(It.IsAny<CoreBusiness.Equipment>()))
                .ReturnsAsync(Result<CoreBusiness.Equipment>.Fail("Database is unavailable."));

            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            var suppilersRepository = new Mock<ISuppilersRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            var locationRepository = new Mock<ILocationRepository>();

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var dto = new EquipmentDto { ModelName = "Dell OptiPlex", SerialNumber = "SN-99999" };

            var result = await useCase.AddEquipmentAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }

        [Fact]
        public async Task GetHardwareTypesAsync_ReturnsMappedDtos()
        {
            var hardwareTypes = new List<CoreBusiness.HardwareType>
            {
                new() { HardwareTypeId = 1, Name = "Laptop", IsActive = true },
                new() { HardwareTypeId = 2, Name = "Monitor", IsActive = false }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            hardwareTypeRepository
                .Setup(r => r.GetHardwareTypesAsync(It.IsAny<Expression<Func<CoreBusiness.HardwareType, bool>>>()))
                .ReturnsAsync(hardwareTypes);
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            var suppilersRepository = new Mock<ISuppilersRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            var locationRepository = new Mock<ILocationRepository>();

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var result = await useCase.GetHardwareTypesAsync(h => h.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.HardwareTypeId == 1 && dto.Name == "Laptop");
            Assert.Contains(result, dto => dto.HardwareTypeId == 2 && dto.Name == "Monitor");
        }

        [Fact]
        public async Task GetManufacturersAsync_ReturnsMappedDtos()
        {
            var manufacturers = new List<Manufacturer>
            {
                new() { ManufacturerId = 1, Name = "Lenovo", IsActive = true },
                new() { ManufacturerId = 2, Name = "Dell", IsActive = true }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            manufacturersRepository
                .Setup(r => r.GetManufacturersAsync(It.IsAny<Expression<Func<Manufacturer, bool>>>()))
                .ReturnsAsync(manufacturers);
            var suppilersRepository = new Mock<ISuppilersRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();

            var locationRepository = new Mock<ILocationRepository>();

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var result = await useCase.GetManufacturersAsync(m => m.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.ManufacturerId == 1 && dto.Name == "Lenovo");
            Assert.Contains(result, dto => dto.ManufacturerId == 2 && dto.Name == "Dell");
        }

        [Fact]
        public async Task GetSuppilersAsync_ReturnsMappedDtos()
        {
            var suppilers = new List<Suppiler>
            {
                new() { SuppilerId = 1, Name = "Alfa Sp. z o.o.", IsActive = true },
                new() { SuppilerId = 2, Name = "Beta S.A.", IsActive = false }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            var suppilersRepository = new Mock<ISuppilersRepository>();
            suppilersRepository
                .Setup(r => r.GetSuppilersAsync(It.IsAny<Expression<Func<Suppiler, bool>>>()))
                .ReturnsAsync(suppilers);
            var invoiceRepository = new Mock<IInvoiceRepository>();

            var locationRepository = new Mock<ILocationRepository>();

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var result = await useCase.GetSuppilersAsync(s => s.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.SuppilerId == 1 && dto.Name == "Alfa Sp. z o.o.");
            Assert.Contains(result, dto => dto.SuppilerId == 2 && dto.Name == "Beta S.A.");
        }

        [Fact]
        public async Task GetInvoicesAsync_ReturnsPassthroughDtos()
        {
            var invoices = new List<InvoiceDto>
            {
                new() { InvoiceId = 1, InvoiceNumber = "FV/2025/001" },
                new() { InvoiceId = 2, InvoiceNumber = "FV/2025/002" }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            var suppilersRepository = new Mock<ISuppilersRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            invoiceRepository
                .Setup(r => r.GetInvoicesAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
                .ReturnsAsync(invoices);

            var locationRepository = new Mock<ILocationRepository>();

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var result = await useCase.GetInvoicesAsync(i => i.InvoiceId > 0);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.InvoiceId == 1 && dto.InvoiceNumber == "FV/2025/001");
            Assert.Contains(result, dto => dto.InvoiceId == 2 && dto.InvoiceNumber == "FV/2025/002");
        }

        [Fact]
        public async Task GetLocationsAsync_ReturnsMappedDtos()
        {
            var locations = new List<Location>
            {
                new() { LocationId = 1, City = "Stryków", Street = "Magazyn Centralny", EquipmentStorage = true, IsActive = true },
                new() { LocationId = 2, City = "Łódź", Street = "Biuro - Srebrzyńska 14", EquipmentStorage = false, IsActive = true }
            };

            var equipmentRepository = new Mock<IEquipmentRepository>();
            var hardwareTypeRepository = new Mock<IHardwareTypeRepository>();
            var manufacturersRepository = new Mock<IManufacturersRepository>();
            var suppilersRepository = new Mock<ISuppilersRepository>();
            var invoiceRepository = new Mock<IInvoiceRepository>();
            var locationRepository = new Mock<ILocationRepository>();
            locationRepository
                .Setup(r => r.GetLocationsAsync(It.IsAny<Expression<Func<Location, bool>>>()))
                .ReturnsAsync(locations);

            var useCase = new AddEquipmentUseCase(
                equipmentRepository.Object,
                hardwareTypeRepository.Object,
                manufacturersRepository.Object,
                suppilersRepository.Object,
                invoiceRepository.Object,
                locationRepository.Object);

            var result = await useCase.GetLocationsAsync(l => l.EquipmentStorage);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.LocationId == 1 && dto.EquipmentStorage);
            Assert.Contains(result, dto => dto.LocationId == 2 && !dto.EquipmentStorage);
        }
    }
}
