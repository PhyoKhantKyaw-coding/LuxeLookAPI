namespace LuxeLookAPI.Services
{
    public static class CurrencyHelper
    {
        private static readonly Dictionary<string, decimal> CurrencyRates = new()
    {
        { "us", 1m },       // USD
        { "uk", 0.79m },    // GBP
        { "thailand", 36.2m }, // THB
        { "japan", 146.5m },   // JPY
        { "myanmar", 2100m },  // MMK
        { "canada", 1.35m }    // CAD
    };

        private static readonly Dictionary<string, string> CurrencySymbols = new()
    {
        { "us", "$" }, 
        { "uk", "£" },
        { "thailand", "฿" },
        { "japan", "¥" },
        { "myanmar", "Ks" },
        { "canada", "C$" }
    };

        public static (decimal convertedValue, string symbol) Convert(string language, decimal value)
        {
            language = language.ToLower();
            if (!CurrencyRates.ContainsKey(language))
                language = "us"; // default fallback to USD

            var rate = CurrencyRates[language];
            var symbol = CurrencySymbols[language];
            return (value * rate, symbol);
        }
    }

}
