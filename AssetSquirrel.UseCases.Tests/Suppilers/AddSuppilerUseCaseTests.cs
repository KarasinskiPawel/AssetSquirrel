using AssetSquirrel.CoreBusiness;
using AssetSquirrel.CoreBusiness.Dto;
using AssetSquirrel.UseCases.PluginInterfaces;
using AssetSquirrel.UseCases.Suppilers;
using Moq;

namespace AssetSquirrel.UseCases.Tests.Suppilers
{
    public class AddSuppilerUseCaseTests
    {
        [Fact]
        public async Task AddSuppilerAsync_MapsDtoToEntity_AndReturnsRepositoryResult()
        {
            var repository = new Mock<ISuppilersRepository>();
            Suppiler? captured = null;
            repository
                .Setup(r => r.AddSuppilerAsync(It.IsAny<Suppiler>()))
                .Callback<Suppiler>(s => captured = s)
                .ReturnsAsync((Suppiler s) => Result<Suppiler>.Ok(s));

            var useCase = new AddSuppilerUseCase(repository.Object);
            var dto = new SuppilerDto { SuppilerId = 1, Name = "X-KOM", Description = "Electronics retailer", IsActive = true };

            var result = await useCase.AddSuppilerAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(captured);
            Assert.Equal(dto.SuppilerId, captured.SuppilerId);
            Assert.Equal(dto.Name, captured.Name);
            Assert.Equal(dto.Description, captured.Description);
            repository.Verify(r => r.AddSuppilerAsync(It.IsAny<Suppiler>()), Times.Once);
        }

        [Fact]
        public async Task AddSuppilerAsync_ReturnsFailureWithMessage_WhenRepositoryReportsFailure()
        {
            var repository = new Mock<ISuppilersRepository>();
            repository.Setup(r => r.AddSuppilerAsync(It.IsAny<Suppiler>())).ReturnsAsync(Result<Suppiler>.Fail("Database is unavailable."));
            var useCase = new AddSuppilerUseCase(repository.Object);
            var result = await useCase.AddSuppilerAsync(new SuppilerDto { Name = "MPC", Description = "Hardware distributor" });
            Assert.False(result.Success);
            Assert.Equal("Database is unavailable.", result.Message);
        }
    }
}
