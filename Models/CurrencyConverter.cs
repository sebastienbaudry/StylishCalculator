using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace StylishCalculator.Models
{
    /// <summary>
    /// Handles currency conversion functionality
    /// </summary>
    public class CurrencyConverter
    {
        #region Properties

        /// <summary>
        /// List of available currencies
        /// </summary>
        public List<string> AvailableCurrencies { get; private set; } = new List<string>
        {
            "USD", "EUR", "GBP", "JPY", "CAD", "AUD", "CHF", "CNY", "INR", "RUB", "IDR"
        };

        /// <summary>
        /// Source currency code
        /// </summary>
        public string FromCurrency { get; set; } = "USD";

        /// <summary>
        /// Target currency code
        /// </summary>
        public string ToCurrency { get; set; } = "EUR";

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
        /// Key is base currency, value is dictionary of target currencies and rates
        /// </summary>
        private Dictionary<string, Dictionary<string, decimal>> _exchangeRateCache = 
            new Dictionary<string, Dictionary<string, decimal>>();

        /// <summary>
        /// HttpClient for API calls
        /// </summary>
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Cache expiration time in minutes
        /// </summary>
        private const int CacheExpirationMinutes = 60;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CurrencyConverter()
        {
            // Initialize with some default exchange rates for offline use
            InitializeDefaultRates();
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
                // Reset error state
                HasError = false;
                ErrorMessage = "";

                // Check if we need to update the exchange rates
                if (NeedToUpdateRates(FromCurrency))
                {
                    await UpdateExchangeRatesAsync(FromCurrency);
                }

                // Get the exchange rate
                decimal exchangeRate = GetExchangeRate(FromCurrency, ToCurrency);

                // Perform the conversion
                ConvertedAmount = Amount * exchangeRate;
            }
            catch (Exception ex)
            {
                SetError($"Conversion error: {ex.Message}");
                ConvertedAmount = 0;
            }
        }

        /// <summary>
        /// Swaps the from and to currencies
        /// </summary>
        public void SwapCurrencies()
        {
            string temp = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = temp;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes default exchange rates for offline use
        /// </summary>
        private void InitializeDefaultRates()
        {
            // Default rates based on USD (as of mid-2023)
            var usdRates = new Dictionary<string, decimal>
            {
                { "USD", 1.0m },
                { "EUR", 0.92m },
                { "GBP", 0.79m },
                { "JPY", 145.0m },
                { "CAD", 1.35m },
                { "AUD", 1.50m },
                { "CHF", 0.90m },
                { "CNY", 7.20m },
                { "INR", 82.0m },
                { "RUB", 90.0m },
                { "IDR", 15600.0m }
            };

            _exchangeRateCache["USD"] = usdRates;
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Checks if we need to update the exchange rates for a given base currency
        /// </summary>
        /// <param name="baseCurrency">The base currency</param>
        /// <returns>True if rates need updating, false otherwise</returns>
        private bool NeedToUpdateRates(string baseCurrency)
        {
            // If we don't have rates for this base currency, or they're expired, update them
            return !_exchangeRateCache.ContainsKey(baseCurrency) ||
                   (DateTime.Now - LastUpdated).TotalMinutes > CacheExpirationMinutes;
        }

        /// <summary>
        /// Updates exchange rates from the API
        /// </summary>
        /// <param name="baseCurrency">The base currency</param>
        private async Task UpdateExchangeRatesAsync(string baseCurrency)
        {
            try
            {
                // In a real implementation, this would call an exchange rate API
                // For now, we'll simulate it by deriving from our USD rates
                await Task.Delay(200); // Simulate API call

                if (baseCurrency != "USD" && _exchangeRateCache.ContainsKey("USD"))
                {
                    // Calculate cross rates based on USD
                    var usdRates = _exchangeRateCache["USD"];
                    var newRates = new Dictionary<string, decimal>();

                    decimal baseToUsd = 1 / usdRates[baseCurrency]; // Inverse of USD to base

                    foreach (var currency in usdRates.Keys)
                    {
                        newRates[currency] = usdRates[currency] * baseToUsd;
                    }

                    _exchangeRateCache[baseCurrency] = newRates;
                }

                LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update exchange rates: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the exchange rate between two currencies
        /// </summary>
        /// <param name="fromCurrency">Source currency</param>
        /// <param name="toCurrency">Target currency</param>
        /// <returns>The exchange rate</returns>
        private decimal GetExchangeRate(string fromCurrency, string toCurrency)
        {
            // If same currency, rate is 1
            if (fromCurrency == toCurrency)
            {
                return 1m;
            }

            // Check if we have the rate
            if (_exchangeRateCache.TryGetValue(fromCurrency, out var rates))
            {
                if (rates.TryGetValue(toCurrency, out var rate))
                {
                    return rate;
                }
            }

            throw new Exception($"Exchange rate not available for {fromCurrency} to {toCurrency}");
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

        #endregion
    }
}