using AssetSquirrel.UseCases.EquipmentUseCase;

namespace AssetSquirrel.UseCases.Tests.Equipment
{
    public class InventoryNumberGeneratorTests
    {
        [Fact]
        public void Next_ReturnsFirstNumber_WhenThereIsNoLastInventoryNumber()
        {
            var result = InventoryNumberGenerator.Next(null);

            Assert.Equal("49100000001", result);
        }

        [Fact]
        public void Next_ReturnsFirstNumber_WhenLastInventoryNumberIsEmpty()
        {
            var result = InventoryNumberGenerator.Next(string.Empty);

            Assert.Equal("49100000001", result);
        }

        [Fact]
        public void Next_IncrementsSuffix_WhenLastInventoryNumberMatchesFormat()
        {
            var result = InventoryNumberGenerator.Next("49100000074");

            Assert.Equal("49100000075", result);
        }

        [Fact]
        public void Next_PadsWithLeadingZeros()
        {
            var result = InventoryNumberGenerator.Next("49100000009");

            Assert.Equal("49100000010", result);
        }

        [Fact]
        public void Next_RestartsFromOne_WhenLastInventoryNumberDoesNotMatchFormat()
        {
            var result = InventoryNumberGenerator.Next("not-a-valid-number");

            Assert.Equal("49100000001", result);
        }
    }
}
