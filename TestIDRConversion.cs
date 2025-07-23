using System;
using System.Threading.Tasks;
using StylishCalculator.Models;

class TestIDRConversion
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== IDR to EUR Conversion Test ===");
        
        var converter = CurrencyConverter.Instance;
        
        // Wait a moment for initialization
        await Task.Delay(2000);
        
        // Set up the conversion
        converter.FromCurrency = converter.AvailableCurrencies.Find(c => c.Code == "IDR");
        converter.ToCurrency = converter.AvailableCurrencies.Find(c => c.Code == "EUR");
        converter.Amount = 10000;
        
        Console.WriteLine($"Converting {converter.Amount} {converter.FromCurrency.Code} to {converter.ToCurrency.Code}");
        
        // Perform conversion
        await converter.ConvertAsync();
        
        Console.WriteLine($"Result: {converter.ConvertedAmount}");
        Console.WriteLine($"Expected: ~0.523 EUR");
        Console.WriteLine($"Has Error: {converter.HasError}");
        if (converter.HasError)
        {
            Console.WriteLine($"Error: {converter.ErrorMessage}");
        }
        
        // Test with different amounts
        Console.WriteLine("\n=== Testing different amounts ===");
        
        decimal[] testAmounts = { 1, 100, 1000, 10000, 100000 };
        
        foreach (var amount in testAmounts)
        {
            converter.Amount = amount;
            await converter.ConvertAsync();
            Console.WriteLine($"{amount:N0} IDR = {converter.ConvertedAmount:F5} EUR");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}