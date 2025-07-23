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
                    ConvertCommand.Execute(null);
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
                    ConvertCommand.Execute(null);
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
                    ConvertCommand.Execute(null);
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
                System.Diagnostics.Debug.WriteLine($"ConversionResult getter called - HasError: {Converter.HasError}, Amount: {Amount}, ConvertedAmount: {ConvertedAmount}");
                Console.WriteLine($"ConversionResult getter called - HasError: {Converter.HasError}, Amount: {Amount}, ConvertedAmount: {ConvertedAmount}");
                
                if (Converter.HasError)
                {
                    return $"Error: {Converter.ErrorMessage}";
                }
                
                if (FromCurrency == null || ToCurrency == null)
                {
                    return "Select currencies to convert";
                }
                
                return $"{Amount:F2} {FromCurrency.Code} = {ConvertedAmount:F5} {ToCurrency.Code}";
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
            System.Diagnostics.Debug.WriteLine("CurrencyViewModel constructor called");
            Console.WriteLine("CurrencyViewModel constructor called");
            
            // Initialize commands
            ConvertCommand = new RelayCommand(async _ => await ConvertAsync());
            SwapCurrenciesCommand = new RelayCommand(_ => SwapCurrencies());
            
            // Trigger initial conversion
            System.Diagnostics.Debug.WriteLine("Triggering initial conversion");
            _ = Task.Run(async () => await ConvertAsync());
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
                System.Diagnostics.Debug.WriteLine("CurrencyViewModel.ConvertAsync called");
                Console.WriteLine("CurrencyViewModel.ConvertAsync called");
                IsBusy = true;
                await Converter.ConvertAsync();
                System.Diagnostics.Debug.WriteLine($"Conversion completed - Result: {ConvertedAmount}");
                Console.WriteLine($"Conversion completed - Result: {ConvertedAmount}");
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
            CurrencyInfo temp = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = temp;
            
            // No need to call Convert as the property setters will trigger it
        }

        #endregion
    }
}