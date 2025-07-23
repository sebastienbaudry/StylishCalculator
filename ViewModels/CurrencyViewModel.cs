using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using StylishCalculator.Models;
using StylishCalculator.Services;

namespace StylishCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for the currency conversion functionality
    /// </summary>
    public class CurrencyViewModel : BaseViewModel
    {
        #region Private Fields
        
        /// <summary>
        /// Configuration instance
        /// </summary>
        private readonly AppConfiguration _config = AppConfiguration.Instance;
        
        /// <summary>
        /// Logging service instance
        /// </summary>
        private readonly LoggingService _logger = LoggingService.Instance;
        
        #endregion
        
        #region Properties

        /// <summary>
        /// The currency converter model (singleton instance)
        /// </summary>
        public CurrencyConverter Converter => CurrencyConverter.Instance;

        /// <summary>
        /// List of available currencies
        /// </summary>
        public List<CurrencyInfo> AvailableCurrencies => Converter.AvailableCurrencies;

        /// <summary>
        /// Source currency
        /// </summary>
        public CurrencyInfo FromCurrency
        {
            get => Converter.FromCurrency;
            set
            {
                if (Converter.FromCurrency != value)
                {
                    Converter.FromCurrency = value;
                    OnPropertyChanged();
                    
                    // Only auto-convert if real-time conversion is enabled
                    if (_config.EnableRealTimeConversion)
                    {
                        ConvertCommand.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// Target currency
        /// </summary>
        public CurrencyInfo ToCurrency
        {
            get => Converter.ToCurrency;
            set
            {
                if (Converter.ToCurrency != value)
                {
                    Converter.ToCurrency = value;
                    OnPropertyChanged();
                    
                    // Only auto-convert if real-time conversion is enabled
                    if (_config.EnableRealTimeConversion)
                    {
                        ConvertCommand.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// Amount to convert
        /// </summary>
        public decimal Amount
        {
            get => Converter.Amount;
            set
            {
                if (Converter.Amount != value)
                {
                    Converter.Amount = value;
                    OnPropertyChanged();
                    
                    // Only auto-convert if real-time conversion is enabled
                    if (_config.EnableRealTimeConversion)
                    {
                        ConvertCommand.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// Converted amount result
        /// </summary>
        public decimal ConvertedAmount
        {
            get => Converter.ConvertedAmount;
        }

        /// <summary>
        /// Formatted string representation of the conversion
        /// </summary>
        public string ConversionResult
        {
            get
            {
                _logger.LogTrace($"ConversionResult getter called - HasError: {Converter.HasError}, Amount: {Amount}, ConvertedAmount: {ConvertedAmount}", "CurrencyViewModel");
                
                if (Converter.HasError)
                {
                    return $"Error: {Converter.ErrorMessage}";
                }
                
                if (FromCurrency == null || ToCurrency == null)
                {
                    return "Select currencies to convert";
                }
                
                // Use configuration for decimal places
                return $"{Amount:F2} {FromCurrency.Code} = {ConvertedAmount.ToString($"F{_config.DisplayDecimalPlaces}")} {ToCurrency.Code}";
            }
        }

        /// <summary>
        /// When exchange rates were last updated
        /// </summary>
        public string LastUpdated
        {
            get
            {
                if (Converter.LastUpdated == DateTime.MinValue)
                {
                    return "Not updated yet";
                }
                
                return $"Last updated: {Converter.LastUpdated:g}";
            }
        }

        /// <summary>
        /// Indicates if there is an ongoing operation
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to perform the conversion
        /// </summary>
        public ICommand ConvertCommand { get; private set; }

        /// <summary>
        /// Command to swap the currencies
        /// </summary>
        public ICommand SwapCurrenciesCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CurrencyViewModel()
        {
            _logger.LogInfo("CurrencyViewModel constructor called", "CurrencyViewModel");
            
            // Initialize commands
            ConvertCommand = new RelayCommand(async _ => await ConvertAsync());
            SwapCurrenciesCommand = new RelayCommand(_ => SwapCurrencies());
            
            // Trigger initial conversion if real-time conversion is enabled
            if (_config.EnableRealTimeConversion)
            {
                _logger.LogDebug("Triggering initial conversion", "CurrencyViewModel");
                _ = Task.Run(async () => await ConvertAsync());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads available currencies
        /// </summary>
        private async Task LoadCurrenciesAsync()
        {
            try
            {
                IsBusy = true;
                await Converter.LoadCurrenciesAsync();
                OnPropertyChanged(nameof(AvailableCurrencies));
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Performs the currency conversion
        /// </summary>
        private async Task ConvertAsync()
        {
            try
            {
                _logger.LogDebug("CurrencyViewModel.ConvertAsync called", "CurrencyViewModel");
                IsBusy = true;
                await Converter.ConvertAsync();
                _logger.LogInfo($"Conversion completed - Result: {ConvertedAmount}", "CurrencyViewModel");
                OnPropertyChanged(nameof(ConvertedAmount));
                OnPropertyChanged(nameof(ConversionResult));
                OnPropertyChanged(nameof(LastUpdated));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during conversion: {ex.Message}", ex, "CurrencyViewModel");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Swaps the from and to currencies
        /// </summary>
        private void SwapCurrencies()
        {
            CurrencyInfo temp = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = temp;
            
            // No need to call Convert as the property setters will trigger it
        }

        #endregion
    }
}