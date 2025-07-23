using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace StylishCalculator.Models
{
    /// <summary>
    /// Handles currency conversion functionality
    /// </summary>
    public class CurrencyConverter
    {
        #region Singleton Pattern
        
        private static CurrencyConverter? _instance;
        private static readonly object _lock = new object();
        
        public static CurrencyConverter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CurrencyConverter();
                        }
                    }
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// List of available currencies
        /// </summary>
        public List<CurrencyInfo> AvailableCurrencies { get; private set; } = new List<CurrencyInfo>();

        /// <summary>
        /// Source currency
        /// </summary>
        public CurrencyInfo FromCurrency { get; set; } = null!;

        /// <summary>
        /// Target currency
        /// </summary>
        public CurrencyInfo ToCurrency { get; set; } = null!;

        /// <summary>
        /// Amount to convert
        /// </summary>
        public decimal Amount { get; set; } = 1;

        /// <summary>
        /// Converted amount result
        /// </summary>
        public decimal ConvertedAmount { get; private set; } = 0;

        /// <summary>
        /// When exchange rates were last updated
        /// </summary>
        public DateTime LastUpdated { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Error message if conversion fails
        /// </summary>
        public string ErrorMessage { get; private set; } = "";

        /// <summary>
        /// Indicates if there was an error during conversion
        /// </summary>
        public bool HasError { get; private set; } = false;

        /// <summary>
        /// Cache of exchange rates to minimize API calls
        /// Key is base currency code, value is dictionary of target currencies and rates
        /// </summary>
        private static Dictionary<string, Dictionary<string, decimal>> _exchangeRateCache =
            new Dictionary<string, Dictionary<string, decimal>>();

        /// <summary>
        /// HttpClient for API calls
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Cache expiration time in minutes
        /// </summary>
        private const int CacheExpirationMinutes = 60;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private CurrencyConverter()
        {
            System.Diagnostics.Debug.WriteLine("CurrencyConverter constructor called");
            Console.WriteLine("CurrencyConverter constructor called");
            LogToFile("CurrencyConverter constructor called");
            
            // Initialize available currencies with flags and country names
            InitializeCurrencies();
            System.Diagnostics.Debug.WriteLine($"Initialized {AvailableCurrencies.Count} currencies");
            Console.WriteLine($"Initialized {AvailableCurrencies.Count} currencies");
            LogToFile($"Initialized {AvailableCurrencies.Count} currencies");
            
            // Set default currencies
            FromCurrency = AvailableCurrencies.Find(c => c.Code == "USD") ?? AvailableCurrencies[0];
            ToCurrency = AvailableCurrencies.Find(c => c.Code == "EUR") ?? AvailableCurrencies[1];
            
            System.Diagnostics.Debug.WriteLine($"Default currencies set - From: {FromCurrency?.Code ?? "NULL"}, To: {ToCurrency?.Code ?? "NULL"}");
            Console.WriteLine($"Default currencies set - From: {FromCurrency?.Code ?? "NULL"}, To: {ToCurrency?.Code ?? "NULL"}");
            
            // Initialize with some default exchange rates for offline use
            InitializeDefaultRates();
            System.Diagnostics.Debug.WriteLine($"Default rates initialized, cache has {_exchangeRateCache.Count} base currencies");
            Console.WriteLine($"Default rates initialized, cache has {_exchangeRateCache.Count} base currencies");
            
            // Load saved rates from file first
            _ = Task.Run(async () => await LoadRatesFromFileAsync());
            
            // Load real exchange rates at startup (async fire-and-forget)
            _ = Task.Run(async () => await LoadRealRatesAtStartupAsync());
            System.Diagnostics.Debug.WriteLine("Started background task to load real rates");
            Console.WriteLine("Started background task to load real rates");
        }

        /// <summary>
        /// Loads real exchange rates at application startup
        /// </summary>
        private async Task LoadRealRatesAtStartupAsync()
        {
            try
            {
                // Load rates for major base currencies that users are likely to select
                var majorCurrencies = new[] { "USD", "EUR", "GBP", "JPY", "CAD", "AUD" };
                
                foreach (var currency in majorCurrencies)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Loading rates for {currency}");
                        await UpdateExchangeRatesAsync(currency);
                        
                        // Small delay between API calls to avoid rate limiting
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load rates for {currency}: {ex.Message}");
                        // Continue with next currency if one fails
                    }
                }
            }
            catch (Exception)
            {
                // If loading fails, we'll continue with default rates
                // Error handling is already done in UpdateExchangeRatesAsync
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads available currencies from the API
        /// </summary>
        public async Task LoadCurrenciesAsync()
        {
            try
            {
                // In a real implementation, this would fetch from an API
                // For now, we'll use the hardcoded list
                await Task.Delay(100); // Simulate API call
                
                // Reset error state
                HasError = false;
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                SetError($"Failed to load currencies: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs the currency conversion
        /// </summary>
        public async Task ConvertAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ConvertAsync called - Amount: {Amount}");
                Console.WriteLine($"ConvertAsync called - Amount: {Amount}");
                LogToFile($"ConvertAsync called - Amount: {Amount}");
                
                // Reset error state
                HasError = false;
                ErrorMessage = "";

                // Ensure we have valid currencies
                if (FromCurrency == null || ToCurrency == null)
                {
                    string errorMsg = $"Invalid currency selection - FromCurrency: {FromCurrency?.Code ?? "NULL"}, ToCurrency: {ToCurrency?.Code ?? "NULL"}";
                    System.Diagnostics.Debug.WriteLine(errorMsg);
                    SetError(errorMsg);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Converting {Amount} from {FromCurrency.Code} to {ToCurrency.Code}");

                // Always try to update rates if we don't have them for this base currency
                // This ensures we have the most current rates
                if (!_exchangeRateCache.ContainsKey(FromCurrency.Code))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"No rates found for {FromCurrency.Code}, fetching from API");
                        await UpdateExchangeRatesAsync(FromCurrency.Code);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Rate update failed: {ex.Message}");
                        // If update fails, continue with cached rates or defaults
                    }
                }
                
                // If we still don't have rates for this currency, try to fetch them again
                if (!_exchangeRateCache.ContainsKey(FromCurrency.Code))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Still no rates for {FromCurrency.Code}, trying again");
                        await UpdateExchangeRatesAsync(FromCurrency.Code);
                        
                        // Give it a moment to complete
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Second rate update failed: {ex.Message}");
                    }
                }

                // Get the exchange rate
                decimal exchangeRate = GetExchangeRate(FromCurrency.Code, ToCurrency.Code);
                System.Diagnostics.Debug.WriteLine($"Exchange rate {FromCurrency.Code} to {ToCurrency.Code}: {exchangeRate}");
                LogToFile($"Exchange rate {FromCurrency.Code} to {ToCurrency.Code}: {exchangeRate}");

                // Perform the conversion
                ConvertedAmount = Amount * exchangeRate;
                System.Diagnostics.Debug.WriteLine($"Conversion result: {Amount} * {exchangeRate} = {ConvertedAmount}");
                LogToFile($"Conversion result: {Amount} * {exchangeRate} = {ConvertedAmount}");
                LogToFile($"FINAL CONVERSION: {Amount} {FromCurrency.Code} = {ConvertedAmount} {ToCurrency.Code}");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Conversion error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(errorMsg);
                SetError(errorMsg);
                ConvertedAmount = 0;
            }
        }

        /// <summary>
        /// Swaps the from and to currencies
        /// </summary>
        public void SwapCurrencies()
        {
            CurrencyInfo temp = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = temp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the list of available currencies with flags and country names
        /// </summary>
        private void InitializeCurrencies()
        {
            AvailableCurrencies = new List<CurrencyInfo>
            {
                new CurrencyInfo("USD", "US Dollar", "United States", "🇺🇸"),
                new CurrencyInfo("EUR", "Euro", "European Union", "🇪🇺"),
                new CurrencyInfo("GBP", "British Pound", "United Kingdom", "🇬🇧"),
                new CurrencyInfo("JPY", "Japanese Yen", "Japan", "🇯🇵"),
                new CurrencyInfo("CAD", "Canadian Dollar", "Canada", "🇨🇦"),
                new CurrencyInfo("AUD", "Australian Dollar", "Australia", "🇦🇺"),
                new CurrencyInfo("CHF", "Swiss Franc", "Switzerland", "🇨🇭"),
                new CurrencyInfo("CNY", "Chinese Yuan", "China", "🇨🇳"),
                new CurrencyInfo("INR", "Indian Rupee", "India", "🇮🇳"),
                new CurrencyInfo("RUB", "Russian Ruble", "Russia", "🇷🇺"),
                new CurrencyInfo("IDR", "Indonesian Rupiah", "Indonesia", "🇮🇩")
            };
        }

        /// <summary>
        /// Initializes default exchange rates for offline use
        /// </summary>
        private void InitializeDefaultRates()
        {
            LogToFile("InitializeDefaultRates() called");
            // Current rates based on USD (as of July 2025)
            var usdRates = new Dictionary<string, decimal>
            {
                { "USD", 1.0m },
                { "EUR", 0.91m },
                { "GBP", 0.78m },
                { "JPY", 155.0m },
                { "CAD", 1.37m },
                { "AUD", 1.48m },
                { "CHF", 0.87m },
                { "CNY", 7.25m },
                { "INR", 83.5m },
                { "RUB", 88.0m },
                { "IDR", 15800.0m }
            };
            
            LogToFile($"USD rates dictionary created with {usdRates.Count} currencies: {string.Join(", ", usdRates.Keys)}");

            _exchangeRateCache["USD"] = usdRates;
            LogToFile($"USD rates added to cache: {usdRates.Count} rates");
            
            // Also create reverse rates for EUR base
            var eurRates = new Dictionary<string, decimal>();
            foreach (var rate in usdRates)
            {
                if (rate.Key == "EUR")
                    eurRates[rate.Key] = 1.0m;
                else if (rate.Key == "USD")
                    eurRates[rate.Key] = 1 / usdRates["EUR"];
                else
                    eurRates[rate.Key] = rate.Value / usdRates["EUR"];
            }
            _exchangeRateCache["EUR"] = eurRates;
            LogToFile($"EUR rates added to cache: {eurRates.Count} rates");
            
            // Create reverse rates for GBP base
            var gbpRates = new Dictionary<string, decimal>();
            foreach (var rate in usdRates)
            {
                if (rate.Key == "GBP")
                    gbpRates[rate.Key] = 1.0m;
                else if (rate.Key == "USD")
                    gbpRates[rate.Key] = 1 / usdRates["GBP"];
                else
                    gbpRates[rate.Key] = rate.Value / usdRates["GBP"];
            }
            _exchangeRateCache["GBP"] = gbpRates;
            LogToFile($"GBP rates added to cache: {gbpRates.Count} rates");
            
            LastUpdated = DateTime.Now;
            LogToFile($"Default rates initialization completed. Total cache entries: {_exchangeRateCache.Count}");
        }

        /// <summary>
        /// Checks if we need to update the exchange rates for a given base currency
        /// </summary>
        /// <param name="baseCurrency">The base currency code</param>
        /// <returns>True if rates need updating, false otherwise</returns>
        private bool NeedToUpdateRates(string baseCurrency)
        {
            // If we don't have rates for this base currency, or they're expired, update them
            bool needsUpdate = !_exchangeRateCache.ContainsKey(baseCurrency) ||
                   (DateTime.Now - LastUpdated).TotalMinutes > CacheExpirationMinutes;
            
            System.Diagnostics.Debug.WriteLine($"NeedToUpdateRates for {baseCurrency}: {needsUpdate}");
            return needsUpdate;
        }

        /// <summary>
        /// Updates exchange rates from the API
        /// </summary>
        /// <param name="baseCurrency">The base currency code</param>
        private async Task UpdateExchangeRatesAsync(string baseCurrency)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"UpdateExchangeRatesAsync called for {baseCurrency}");
                
                // Try to fetch real exchange rates from a free API
                // The FetchRealExchangeRatesAsync method already stores the rates in the cache
                await FetchRealExchangeRatesAsync(baseCurrency);
                
                // No need for cross-rate calculation here since the API provides direct rates
                // The FetchRealExchangeRatesAsync method handles storing the rates
                
                LastUpdated = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"Exchange rates updated successfully for {baseCurrency}");
            }
            catch (Exception ex)
            {
                // If API fails, fall back to default rates
                System.Diagnostics.Debug.WriteLine($"UpdateExchangeRatesAsync failed: {ex.Message}");
                SetError($"Failed to update exchange rates, using cached rates: {ex.Message}");
            }
        }

        /// <summary>
        /// Fetches real exchange rates from freecurrencyapi.com
        /// </summary>
        /// <param name="baseCurrency">The base currency code</param>
        private async Task FetchRealExchangeRatesAsync(string baseCurrency)
        {
            try
            {
                // Using freecurrencyapi.com with provided API key - get all currencies at once
                string apiKey = "fca_live_S38vx3saYCkKXDPiePO2ym3ZN8ltZjVyv9daIrA8";
                string currencies = "USD,EUR,GBP,JPY,CAD,AUD,CHF,CNY,INR,RUB,IDR";
                string apiUrl = $"https://api.freecurrencyapi.com/v1/latest?apikey={apiKey}&base_currency={baseCurrency}&currencies={currencies}";
                
                System.Diagnostics.Debug.WriteLine($"Fetching rates from: {apiUrl}");
                LogToFile($"Fetching rates from API for {baseCurrency}");
                
                var response = await _httpClient.GetStringAsync(apiUrl);
                System.Diagnostics.Debug.WriteLine($"API Response: {response}");
                LogToFile($"API Response received: {response.Length} characters");
                
                var jsonDoc = JsonDocument.Parse(response);
                
                if (jsonDoc.RootElement.TryGetProperty("data", out var dataElement))
                {
                    var rates = new Dictionary<string, decimal>();
                    
                    // Add the base currency with rate 1.0
                    rates[baseCurrency] = 1.0m;
                    
                    int rateCount = 0;
                    foreach (var rate in dataElement.EnumerateObject())
                    {
                        if (decimal.TryParse(rate.Value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal rateValue))
                        {
                            rates[rate.Name] = rateValue; // Keep full precision for calculations
                            rateCount++;
                            LogToFile($"API rate: {baseCurrency} -> {rate.Name} = {rateValue}");
                        }
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Successfully loaded {rateCount} exchange rates for {baseCurrency}");
                    LogToFile($"API returned {rateCount} real rates for {baseCurrency}: {string.Join(", ", rates.Keys)}");
                    
                    // Only preserve existing rates if API didn't return enough rates
                    if (rateCount < 5 && _exchangeRateCache.ContainsKey(baseCurrency))
                    {
                        var existingRates = _exchangeRateCache[baseCurrency];
                        foreach (var existingRate in existingRates)
                        {
                            if (!rates.ContainsKey(existingRate.Key))
                            {
                                rates[existingRate.Key] = existingRate.Value;
                                LogToFile($"Preserved existing rate: {baseCurrency} -> {existingRate.Key} = {existingRate.Value}");
                            }
                        }
                    }
                    
                    _exchangeRateCache[baseCurrency] = rates;
                    LogToFile($"Final rates for {baseCurrency}: {rates.Count} currencies: {string.Join(", ", rates.Keys)}");
                    
                    // Save rates to file for offline use
                    await SaveRatesToFileAsync();
                    
                    HasError = false;
                    ErrorMessage = "";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No 'data' property found in API response");
                    LogToFile("ERROR: No 'data' property found in API response");
                    throw new Exception("Invalid API response format - no 'data' property found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API call failed: {ex.Message}");
                LogToFile($"ERROR: API call failed: {ex.Message}");
                throw new Exception($"API call failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the exchange rate between two currencies
        /// </summary>
        /// <param name="fromCurrency">Source currency code</param>
        /// <param name="toCurrency">Target currency code</param>
        /// <returns>The exchange rate</returns>
        private decimal GetExchangeRate(string fromCurrency, string toCurrency)
        {
            System.Diagnostics.Debug.WriteLine($"GetExchangeRate called: {fromCurrency} -> {toCurrency}");
            System.Diagnostics.Debug.WriteLine($"Cache contains {_exchangeRateCache.Count} base currencies: {string.Join(", ", _exchangeRateCache.Keys)}");
            LogToFile($"GetExchangeRate called: {fromCurrency} -> {toCurrency}");
            LogToFile($"Cache contains {_exchangeRateCache.Count} base currencies: {string.Join(", ", _exchangeRateCache.Keys)}");
            
            // If same currency, rate is 1
            if (fromCurrency == toCurrency)
            {
                System.Diagnostics.Debug.WriteLine("Same currency, returning 1.0");
                return 1m;
            }

            // Check if we have the rate directly
            if (_exchangeRateCache.TryGetValue(fromCurrency, out var rates))
            {
                System.Diagnostics.Debug.WriteLine($"Found rates for {fromCurrency}, contains {rates.Count} currencies: {string.Join(", ", rates.Keys)}");
                LogToFile($"Found rates for {fromCurrency}, contains {rates.Count} currencies: {string.Join(", ", rates.Keys)}");
                if (rates.TryGetValue(toCurrency, out var rate))
                {
                    System.Diagnostics.Debug.WriteLine($"Direct rate found: {fromCurrency} -> {toCurrency} = {rate}");
                    LogToFile($"Direct rate found: {fromCurrency} -> {toCurrency} = {rate}");
                    LogToFile($"FINAL RESULT: GetExchangeRate({fromCurrency} -> {toCurrency}) returning {rate}");
                    return rate;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No direct rate found for {toCurrency} in {fromCurrency} rates");
                    LogToFile($"No direct rate found for {toCurrency} in {fromCurrency} rates");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No rates found for base currency {fromCurrency}");
            }

            // Try to calculate cross rate via USD
            if (fromCurrency != "USD" && toCurrency != "USD" && _exchangeRateCache.TryGetValue("USD", out var usdRates))
            {
                System.Diagnostics.Debug.WriteLine($"Attempting cross-rate calculation via USD");
                if (usdRates.TryGetValue(fromCurrency, out var fromRate) && usdRates.TryGetValue(toCurrency, out var toRate))
                {
                    // Cross rate calculation: (USD/from) * (to/USD) = to/from
                    decimal crossRate = toRate / fromRate; // Keep full precision
                    System.Diagnostics.Debug.WriteLine($"Cross rate calculated: {toRate} / {fromRate} = {crossRate}");
                    LogToFile($"Cross rate calculated: {toRate} / {fromRate} = {crossRate}");
                    return crossRate;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Cross-rate calculation failed - fromRate found: {usdRates.ContainsKey(fromCurrency)}, toRate found: {usdRates.ContainsKey(toCurrency)}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Cross-rate calculation not applicable or USD rates not available");
            }

            // If we still don't have a rate, try to use default rates as fallback
            if (_exchangeRateCache.ContainsKey("USD"))
            {
                var fallbackUsdRates = _exchangeRateCache["USD"];
                if (fromCurrency == "USD" && fallbackUsdRates.ContainsKey(toCurrency))
                {
                    System.Diagnostics.Debug.WriteLine($"Using USD base rate for {toCurrency}: {fallbackUsdRates[toCurrency]}");
                    return fallbackUsdRates[toCurrency];
                }
                else if (toCurrency == "USD" && fallbackUsdRates.ContainsKey(fromCurrency))
                {
                    decimal rate = 1m / fallbackUsdRates[fromCurrency]; // Keep full precision
                    System.Diagnostics.Debug.WriteLine($"Using inverse USD rate for {fromCurrency}: {rate}");
                    LogToFile($"Using inverse USD rate for {fromCurrency}: {rate}");
                    return rate;
                }
                else if (fallbackUsdRates.ContainsKey(fromCurrency) && fallbackUsdRates.ContainsKey(toCurrency))
                {
                    // Calculate cross rate via USD
                    decimal crossRate = fallbackUsdRates[toCurrency] / fallbackUsdRates[fromCurrency]; // Keep full precision
                    System.Diagnostics.Debug.WriteLine($"Using USD cross rate: {fallbackUsdRates[toCurrency]} / {fallbackUsdRates[fromCurrency]} = {crossRate}");
                    LogToFile($"Using USD cross rate: {fallbackUsdRates[toCurrency]} / {fallbackUsdRates[fromCurrency]} = {crossRate}");
                    return crossRate;
                }
            }
            
            // Try EUR as fallback if USD doesn't work
            if (_exchangeRateCache.ContainsKey("EUR"))
            {
                var fallbackEurRates = _exchangeRateCache["EUR"];
                if (fallbackEurRates.ContainsKey(fromCurrency) && fallbackEurRates.ContainsKey(toCurrency))
                {
                    decimal crossRate = fallbackEurRates[toCurrency] / fallbackEurRates[fromCurrency]; // Keep full precision
                    System.Diagnostics.Debug.WriteLine($"Using EUR cross rate: {fallbackEurRates[toCurrency]} / {fallbackEurRates[fromCurrency]} = {crossRate}");
                    LogToFile($"Using EUR cross rate: {fallbackEurRates[toCurrency]} / {fallbackEurRates[fromCurrency]} = {crossRate}");
                    return crossRate;
                }
            }
            
            // If we still don't have a rate, return a default rate to prevent errors
            string errorMsg = $"Exchange rate not available for {fromCurrency} to {toCurrency}, using default rate";
            System.Diagnostics.Debug.WriteLine(errorMsg);
            Console.WriteLine($"ERROR: {errorMsg}");
            LogToFile($"ERROR: {errorMsg}");
            SetError(errorMsg);
            return 1m; // Default to 1:1 ratio to prevent crashes
        }

        /// <summary>
        /// Sets an error state with the specified message
        /// </summary>
        /// <param name="message">The error message</param>
        private void SetError(string message)
        {
            HasError = true;
            ErrorMessage = message;
        }

        /// <summary>
        /// Logs a message to a debug file
        /// </summary>
        /// <param name="message">The message to log</param>
        private void LogToFile(string message)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}\n";
                File.AppendAllText("currency_debug.log", logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }

        /// <summary>
        /// Saves current exchange rates to a file for offline use
        /// </summary>
        private async Task SaveRatesToFileAsync()
        {
            try
            {
                var ratesData = new Dictionary<string, object>
                {
                    ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["rates"] = _exchangeRateCache
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    // Preserve full decimal precision for very small numbers
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
                };
                
                string json = JsonSerializer.Serialize(ratesData, options);
                await File.WriteAllTextAsync("exchange_rates.json", json);
                LogToFile($"Saved {_exchangeRateCache.Count} base currencies to exchange_rates.json with full precision");
            }
            catch (Exception ex)
            {
                LogToFile($"Failed to save rates to file: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads exchange rates from file for offline use
        /// </summary>
        private async Task LoadRatesFromFileAsync()
        {
            try
            {
                if (File.Exists("exchange_rates.json"))
                {
                    string json = await File.ReadAllTextAsync("exchange_rates.json");
                    var ratesData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    if (ratesData != null && ratesData.ContainsKey("rates"))
                    {
                        var ratesElement = (JsonElement)ratesData["rates"];
                        var loadedRates = new Dictionary<string, Dictionary<string, decimal>>();
                        
                        foreach (var baseCurrency in ratesElement.EnumerateObject())
                        {
                            var currencyRates = new Dictionary<string, decimal>();
                            foreach (var rate in baseCurrency.Value.EnumerateObject())
                            {
                                if (rate.Value.TryGetDecimal(out decimal rateValue))
                                {
                                    currencyRates[rate.Name] = rateValue;
                                }
                            }
                            loadedRates[baseCurrency.Name] = currencyRates;
                        }
                        
                        // Merge loaded rates with existing cache, preferring existing (newer) rates
                        foreach (var loadedRate in loadedRates)
                        {
                            if (!_exchangeRateCache.ContainsKey(loadedRate.Key))
                            {
                                _exchangeRateCache[loadedRate.Key] = loadedRate.Value;
                            }
                        }
                        
                        LogToFile($"Loaded {loadedRates.Count} base currencies from exchange_rates.json");
                        
                        if (ratesData.ContainsKey("timestamp"))
                        {
                            LogToFile($"File rates timestamp: {ratesData["timestamp"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogToFile($"Failed to load rates from file: {ex.Message}");
            }
        }

        #endregion
    }
}