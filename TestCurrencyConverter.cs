using System;
using System.Threading.Tasks;
using StylishCalculator.Models;

namespace StylishCalculator
{
    class TestCurrencyConverter
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Testing Currency Converter...");
            
            var converter = CurrencyConverter.Instance;
            
            // Wait a moment for initialization
            await Task.Delay(1000);
            
            Console.WriteLine($"Available currencies: {converter.AvailableCurrencies.Count}");
            Console.WriteLine($"From Currency: {converter.FromCurrency?.Code ?? "NULL"}");
            Console.WriteLine($"To Currency: {converter.ToCurrency?.Code ?? "NULL"}");
            Console.WriteLine($"Amount: {converter.Amount}");
            
            // Test conversion
            converter.Amount = 100;
            await converter.ConvertAsync();
            
            Console.WriteLine($"Conversion result: {converter.ConvertedAmount}");
            Console.WriteLine($"Has error: {converter.HasError}");
            Console.WriteLine($"Error message: {converter.ErrorMessage}");
            Console.WriteLine($"Last updated: {converter.LastUpdated}");
            
            // Wait for API calls to complete
            Console.WriteLine("Waiting for API calls to complete...");
            await Task.Delay(5000);
            
            // Test conversion again
            Console.WriteLine("\nTesting conversion after API calls...");
            await converter.ConvertAsync();
            
            Console.WriteLine($"Conversion result: {converter.ConvertedAmount}");
            Console.WriteLine($"Has error: {converter.HasError}");
            Console.WriteLine($"Error message: {converter.ErrorMessage}");
            Console.WriteLine($"Last updated: {converter.LastUpdated}");
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}