using System;
using System.Threading.Tasks;
using StylishCalculator.Models;

class TestSimpleConversion
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Simple Currency Conversion Test ===");
        
        var converter = CurrencyConverter.Instance;
        
        Console.WriteLine($"FromCurrency: {converter.FromCurrency?.Code}");
        Console.WriteLine($"ToCurrency: {converter.ToCurrency?.Code}");
        Console.WriteLine($"Amount: {converter.Amount}");
        
        // Test with default amount (1)
        Console.WriteLine("\n--- Testing conversion with amount 1 ---");
        await converter.ConvertAsync();
        
        Console.WriteLine($"ConvertedAmount: {converter.ConvertedAmount}");
        Console.WriteLine($"HasError: {converter.HasError}");
        Console.WriteLine($"ErrorMessage: {converter.ErrorMessage}");
        
        // Test with amount 100
        Console.WriteLine("\n--- Testing conversion with amount 100 ---");
        converter.Amount = 100;
        await converter.ConvertAsync();
        
        Console.WriteLine($"ConvertedAmount: {converter.ConvertedAmount}");
        Console.WriteLine($"HasError: {converter.HasError}");
        Console.WriteLine($"ErrorMessage: {converter.ErrorMessage}");
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}