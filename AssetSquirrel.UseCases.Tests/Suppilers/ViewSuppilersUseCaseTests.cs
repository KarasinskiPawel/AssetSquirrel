using System.Linq.Expressions;
using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Suppilers
{
    public class ViewSuppilersUseCaseTests
    {
        [Fact]
        public async Task GetSuppilersAsync_ReturnsMappedDtos()
        {
            var suppilers = new List<Suppiler>
            {
                new() { SuppilerId = 1, Name = "X-KOM", Description = "Electronics retailer", IsActive = true },
                new() { SuppilerId = 2, Name = "MPC", Description = "Hardware distributor", IsActive = false }
            };

            var repository = new Mock<ISuppilersRepository>();
            repository
                .Setup(r => r.GetSuppilersAsync(It.IsAny<Expression<Func<Suppiler, bool>>>()))
                .ReturnsAsync(suppilers);

            var useCase = new ViewSuppilersUseCase(repository.Object);

            var result = await useCase.GetSuppilersAsync(s => s.IsActive);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, dto => dto.SuppilerId == 1 && dto.Name == "X-KOM" && dto.Description == "Electronics retailer");
            Assert.Contains(result, dto => dto.SuppilerId == 2 && dto.Name == "MPC");
        }

        [Fact]
        public async Task UpdateSuppiler_DelegatesToRepositoryUpdateSuppilerAsync()
        {
            var repository = new Mock<ISuppilersRepository>();
            repository
                .Setup(r => r.UpdateSuppilerAsync(It.IsAny<Suppiler>()))
                .ReturnsAsync((Suppiler s) => Result<Suppiler>.Ok(s));

            var useCase = new ViewSuppilersUseCase(repository.Object);
            var dto = new SuppilerDto { SuppilerId = 7, Name = "Lantre" };

            var result = await useCase.UpdateSuppiler(dto);

            Assert.True(result.Success);
            repository.Verify(r => r.UpdateSuppilerAsync(It.Is<Suppiler>(s => s.SuppilerId == 7)), Times.Once);
        }

        [Fact]
        public async Task DeleteSuppiler_MapsDtoToEntity_AndDelegatesToRepository()
        {
            var repository = new Mock<ISuppilersRepository>();
            Suppiler? captured = null;
            repository
                .Setup(r => r.DeleteSuppilerAsync(It.IsAny<Suppiler>()))
                .Callback<Suppiler>(s => captured = s)
                .ReturnsAsync((Suppiler s) => Result<Suppiler>.Ok(s));

            var useCase = new ViewSuppilersUseCase(repository.Object);
            var dto = new SuppilerDto { SuppilerId = 5, Name = "X-KOM" };

            var result = await useCase.DeleteSuppiler(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(5, captured.SuppilerId);
            repository.Verify(r => r.DeleteSuppilerAsync(It.IsAny<Suppiler>()), Times.Once);
        }
    }
}
