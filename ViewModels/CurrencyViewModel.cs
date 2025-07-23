using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using StylishCalculator.Models;

namespace StylishCalculator.ViewModels
{
    /// <summary>
    /// ViewModel for the currency conversion functionality
    /// </summary>
    public class CurrencyViewModel : BaseViewModel
    {
        #region Properties

        private CurrencyConverter _converter = new CurrencyConverter();
        
        /// <summary>
        /// The currency converter model
        /// </summary>
        public CurrencyConverter Converter => _converter;

        /// <summary>
        /// List of available currencies
        /// </summary>
        public List<string> AvailableCurrencies => _converter.AvailableCurrencies;

        /// <summary>
        /// Source currency code
        /// </summary>
        public string FromCurrency
        {
            get => _converter.FromCurrency;
            set
            {
                if (_converter.FromCurrency != value)
                {
                    _converter.FromCurrency = value;
                    OnPropertyChanged();
                    ConvertCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Target currency code
        /// </summary>
        public string ToCurrency
        {
            get => _converter.ToCurrency;
            set
            {
                if (_converter.ToCurrency != value)
                {
                    _converter.ToCurrency = value;
                    OnPropertyChanged();
                    ConvertCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Amount to convert
        /// </summary>
        public decimal Amount
        {
            get => _converter.Amount;
            set
            {
                if (_converter.Amount != value)
                {
                    _converter.Amount = value;
                    OnPropertyChanged();
                    ConvertCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Converted amount result
        /// </summary>
        public decimal ConvertedAmount
        {
            get => _converter.ConvertedAmount;
        }

        /// <summary>
        /// Formatted string representation of the conversion
        /// </summary>
        public string ConversionResult
        {
            get
            {
                if (_converter.HasError)
                {
                    return $"Error: {_converter.ErrorMessage}";
                }
                
                return $"{Amount} {FromCurrency} = {ConvertedAmount} {ToCurrency}";
            }
        }

        /// <summary>
        /// When exchange rates were last updated
        /// </summary>
        public string LastUpdated
        {
            get
            {
                if (_converter.LastUpdated == DateTime.MinValue)
                {
                    return "Not updated yet";
                }
                
                return $"Last updated: {_converter.LastUpdated:g}";
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
            // Initialize commands
            ConvertCommand = new RelayCommand(async _ => await ConvertAsync());
            SwapCurrenciesCommand = new RelayCommand(_ => SwapCurrencies());

            // Load currencies
            Task.Run(async () => await LoadCurrenciesAsync());
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
                await _converter.LoadCurrenciesAsync();
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
                IsBusy = true;
                await _converter.ConvertAsync();
                OnPropertyChanged(nameof(ConvertedAmount));
                OnPropertyChanged(nameof(ConversionResult));
                OnPropertyChanged(nameof(LastUpdated));
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
            string temp = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = temp;
            
            // No need to call Convert as the property setters will trigger it
        }

        #endregion
    }
}