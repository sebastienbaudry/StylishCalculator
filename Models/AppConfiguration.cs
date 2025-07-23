using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StylishCalculator.Models
{
    /// <summary>
    /// Central configuration class containing all application parameters, constants, and settings
    /// </summary>
    public class AppConfiguration
    {
        #region Singleton Pattern
        
        private static AppConfiguration? _instance;
        private static readonly object _lock = new object();
        
        public static AppConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = LoadConfiguration();
                        }
                    }
                }
                return _instance;
            }
        }
        
        #endregion

        #region Application Information
        
        /// <summary>
        /// Application name
        /// </summary>
        public string ApplicationName { get; set; } = "Stylish Calculator";
        
        /// <summary>
        /// Application version
        /// </summary>
        public string Version { get; set; } = "1.0.0";
        
        /// <summary>
        /// Application author
        /// </summary>
        public string Author { get; set; } = "Calculator Team";
        
        #endregion

        #region Logging Configuration
        
        /// <summary>
        /// Current logging level for the application
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel LogLevel { get; set; } = LogLevel.Info;
        
        /// <summary>
        /// Enable file logging
        /// </summary>
        public bool EnableFileLogging { get; set; } = true;
        
        /// <summary>
        /// Enable console logging
        /// </summary>
        public bool EnableConsoleLogging { get; set; } = true;
        
        /// <summary>
        /// Enable debug output logging
        /// </summary>
        public bool EnableDebugLogging { get; set; } = true;
        
        /// <summary>
        /// Log file name
        /// </summary>
        public string LogFileName { get; set; } = "currency_debug.log";
        
        /// <summary>
        /// Maximum log file size in MB before rotation
        /// </summary>
        public int MaxLogFileSizeMB { get; set; } = 10;
        
        /// <summary>
        /// Number of log files to keep during rotation
        /// </summary>
        public int LogFileRetentionCount { get; set; } = 5;
        
        #endregion

        #region Currency API Configuration
        
        /// <summary>
        /// Currency API key for freecurrencyapi.com
        /// </summary>
        public string CurrencyApiKey { get; set; } = "fca_live_S38vx3saYCkKXDPiePO2ym3ZN8ltZjVyv9daIrA8";
        
        /// <summary>
        /// Base URL for the currency API
        /// </summary>
        public string CurrencyApiBaseUrl { get; set; } = "https://api.freecurrencyapi.com/v1/latest";
        
        /// <summary>
        /// Supported currencies list
        /// </summary>
        public string SupportedCurrencies { get; set; } = "USD,EUR,GBP,JPY,CAD,AUD,CHF,CNY,INR,RUB,IDR";
        
        /// <summary>
        /// HTTP timeout for API calls in seconds
        /// </summary>
        public int ApiTimeoutSeconds { get; set; } = 30;
        
        /// <summary>
        /// Number of retry attempts for failed API calls
        /// </summary>
        public int ApiRetryAttempts { get; set; } = 3;
        
        /// <summary>
        /// Delay between API retry attempts in milliseconds
        /// </summary>
        public int ApiRetryDelayMs { get; set; } = 1000;
        
        #endregion

        #region Exchange Rate Caching
        
        /// <summary>
        /// Cache expiration time in minutes
        /// </summary>
        public int CacheExpirationMinutes { get; set; } = 60;
        
        /// <summary>
        /// Exchange rates file name
        /// </summary>
        public string ExchangeRatesFileName { get; set; } = "exchange_rates.json";
        
        /// <summary>
        /// Enable persistent storage of exchange rates
        /// </summary>
        public bool EnablePersistentStorage { get; set; } = true;
        
        /// <summary>
        /// Load exchange rates at startup
        /// </summary>
        public bool LoadRatesAtStartup { get; set; } = true;
        
        /// <summary>
        /// Major currencies to preload at startup
        /// </summary>
        public string[] MajorCurrencies { get; set; } = { "USD", "EUR", "GBP", "JPY", "CAD", "AUD" };
        
        /// <summary>
        /// Delay between API calls to avoid rate limiting (milliseconds)
        /// </summary>
        public int ApiCallDelayMs { get; set; } = 100;
        
        #endregion

        #region UI Configuration
        
        /// <summary>
        /// Default source currency
        /// </summary>
        public string DefaultFromCurrency { get; set; } = "USD";
        
        /// <summary>
        /// Default target currency
        /// </summary>
        public string DefaultToCurrency { get; set; } = "EUR";
        
        /// <summary>
        /// Number of decimal places to display for converted amounts
        /// </summary>
        public int DisplayDecimalPlaces { get; set; } = 5;
        
        /// <summary>
        /// Enable real-time conversion as user types
        /// </summary>
        public bool EnableRealTimeConversion { get; set; } = true;
        
        /// <summary>
        /// Enable amount synchronization between calculator and currency converter
        /// </summary>
        public bool EnableAmountSynchronization { get; set; } = true;
        
        /// <summary>
        /// Window width
        /// </summary>
        public double WindowWidth { get; set; } = 400;
        
        /// <summary>
        /// Window height
        /// </summary>
        public double WindowHeight { get; set; } = 600;
        
        /// <summary>
        /// Enable window state persistence
        /// </summary>
        public bool RememberWindowState { get; set; } = true;
        
        #endregion

        #region Calculator Configuration
        
        /// <summary>
        /// Maximum number of digits in calculator display
        /// </summary>
        public int MaxCalculatorDigits { get; set; } = 15;
        
        /// <summary>
        /// Enable scientific notation for large numbers
        /// </summary>
        public bool EnableScientificNotation { get; set; } = true;
        
        /// <summary>
        /// Threshold for switching to scientific notation
        /// </summary>
        public double ScientificNotationThreshold { get; set; } = 1e10;
        
        #endregion

        #region File Paths
        
        /// <summary>
        /// Configuration file name
        /// </summary>
        public const string ConfigFileName = "app_config.json";
        
        #endregion

        #region Configuration Management
        
        /// <summary>
        /// Load configuration from file or create default
        /// </summary>
        private static AppConfiguration LoadConfiguration()
        {
            try
            {
                if (File.Exists(ConfigFileName))
                {
                    string json = File.ReadAllText(ConfigFileName);
                    var config = JsonSerializer.Deserialize<AppConfiguration>(json, GetJsonOptions());
                    return config ?? new AppConfiguration();
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with default configuration
                System.Diagnostics.Debug.WriteLine($"Failed to load configuration: {ex.Message}");
            }
            
            // Return default configuration and save it
            var defaultConfig = new AppConfiguration();
            defaultConfig.SaveConfiguration();
            return defaultConfig;
        }
        
        /// <summary>
        /// Save current configuration to file
        /// </summary>
        public void SaveConfiguration()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, GetJsonOptions());
                File.WriteAllText(ConfigFileName, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save configuration: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get JSON serialization options
        /// </summary>
        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };
        }
        
        /// <summary>
        /// Reset configuration to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            var defaultConfig = new AppConfiguration();
            
            // Copy all properties from default
            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    property.SetValue(this, property.GetValue(defaultConfig));
                }
            }
            
            SaveConfiguration();
        }
        
        #endregion
    }
}