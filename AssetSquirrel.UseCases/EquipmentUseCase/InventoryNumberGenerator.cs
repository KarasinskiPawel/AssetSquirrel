namespace AssetSquirrel.UseCases.EquipmentUseCase
{
    // Format confirmed with Paweł (_specs/equipment-inventory-number-column.md):
    // "491" followed by 8 digits, next number = last one in the table + 1.
    public static class InventoryNumberGenerator
    {
        private const string Prefix = "491";
        private const int SuffixLength = 8;

        public static string Next(string? lastInventoryNumber)
        {
            long nextSuffix = 1;

            if (!string.IsNullOrEmpty(lastInventoryNumber)
                && lastInventoryNumber.Length == Prefix.Length + SuffixLength
                && lastInventoryNumber.StartsWith(Prefix)
                && long.TryParse(lastInventoryNumber.Substring(Prefix.Length), out var lastSuffix))
            {
                nextSuffix = lastSuffix + 1;
            }

            return Prefix + nextSuffix.ToString().PadLeft(SuffixLength, '0');
        }
    }
}
