using System;
using System.Windows.Input;
using StylishCalculator.Models;

namespace StylishCalculator.ViewModels
{
    /// <summary>
    /// Main ViewModel for the calculator application
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        private CalculatorEngine _calculator = new CalculatorEngine();
        
        /// <summary>
        /// The calculator engine
        /// </summary>
        public CalculatorEngine Calculator => _calculator;

        private CurrencyViewModel _currencyVM = new CurrencyViewModel();
        
        /// <summary>
        /// The currency converter ViewModel
        /// </summary>
        public CurrencyViewModel CurrencyVM => _currencyVM;

        private bool _isCurrencyPanelVisible = false;
        
        /// <summary>
        /// Indicates if the currency conversion panel is visible
        /// </summary>
        public bool IsCurrencyPanelVisible
        {
            get => _isCurrencyPanelVisible;
            set => SetProperty(ref _isCurrencyPanelVisible, value);
        }

        /// <summary>
        /// Current input displayed on the calculator
        /// </summary>
        public string CurrentInput => _calculator.CurrentInput;

        /// <summary>
        /// Current expression being built
        /// </summary>
        public string CurrentExpression => _calculator.CurrentExpression;

        /// <summary>
        /// Indicates if the calculator is in an error state
        /// </summary>
        public bool HasError => _calculator.HasError;

        /// <summary>
        /// Error message if HasError is true
        /// </summary>
        public string ErrorMessage => _calculator.ErrorMessage;

        #endregion

        #region Commands

        /// <summary>
        /// Command to handle digit button clicks
        /// </summary>
        public ICommand DigitCommand { get; private set; }

        /// <summary>
        /// Command to handle decimal point button click
        /// </summary>
        public ICommand DecimalPointCommand { get; private set; }

        /// <summary>
        /// Command to handle operation button clicks
        /// </summary>
        public ICommand OperationCommand { get; private set; }

        /// <summary>
        /// Command to handle equals button click
        /// </summary>
        public ICommand EqualsCommand { get; private set; }

        /// <summary>
        /// Command to handle clear button click
        /// </summary>
        public ICommand ClearCommand { get; private set; }

        /// <summary>
        /// Command to handle clear entry button click
        /// </summary>
        public ICommand ClearEntryCommand { get; private set; }

        /// <summary>
        /// Command to handle backspace button click
        /// </summary>
        public ICommand BackspaceCommand { get; private set; }

        /// <summary>
        /// Command to handle toggle sign button click
        /// </summary>
        public ICommand ToggleSignCommand { get; private set; }

        /// <summary>
        /// Command to handle percentage button click
        /// </summary>
        public ICommand PercentageCommand { get; private set; }

        /// <summary>
        /// Command to toggle the currency conversion panel
        /// </summary>
        public ICommand ToggleCurrencyPanelCommand { get; private set; }

        /// <summary>
        /// Command to send the current result to the currency converter
        /// </summary>
        public ICommand SendToCurrencyCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            // Initialize commands
            DigitCommand = new RelayCommand(param => AppendDigit(param as string ?? string.Empty));
            DecimalPointCommand = new RelayCommand(_ => AppendDecimalPoint());
            OperationCommand = new RelayCommand(param => SetOperation(param as string ?? string.Empty));
            EqualsCommand = new RelayCommand(_ => CalculateResult());
            ClearCommand = new RelayCommand(_ => Clear());
            ClearEntryCommand = new RelayCommand(_ => ClearEntry());
            BackspaceCommand = new RelayCommand(_ => Backspace());
            ToggleSignCommand = new RelayCommand(_ => ToggleSign());
            PercentageCommand = new RelayCommand(_ => CalculatePercentage());
            ToggleCurrencyPanelCommand = new RelayCommand(_ => ToggleCurrencyPanel());
            SendToCurrencyCommand = new RelayCommand(_ => SendToCurrency());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Appends a digit to the current input
        /// </summary>
        /// <param name="digit">The digit to append</param>
        private void AppendDigit(string digit)
        {
            _calculator.AppendDigit(digit);
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Appends a decimal point to the current input
        /// </summary>
        private void AppendDecimalPoint()
        {
            _calculator.AppendDecimalPoint();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Sets the operation to perform
        /// </summary>
        /// <param name="operation">The operation symbol</param>
        private void SetOperation(string operation)
        {
            _calculator.SetOperation(operation);
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Calculates the result of the current expression
        /// </summary>
        private void CalculateResult()
        {
            _calculator.CalculateResult();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Clears all calculator state
        /// </summary>
        private void Clear()
        {
            _calculator.Clear();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Clears only the current entry
        /// </summary>
        private void ClearEntry()
        {
            _calculator.ClearEntry();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Removes the last character from the current input
        /// </summary>
        private void Backspace()
        {
            _calculator.Backspace();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Toggles the sign of the current input
        /// </summary>
        private void ToggleSign()
        {
            _calculator.ToggleSign();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Calculates the percentage of the current input
        /// </summary>
        private void CalculatePercentage()
        {
            _calculator.CalculatePercentage();
            NotifyCalculatorPropertiesChanged();
        }

        /// <summary>
        /// Toggles the visibility of the currency conversion panel
        /// </summary>
        private void ToggleCurrencyPanel()
        {
            IsCurrencyPanelVisible = !IsCurrencyPanelVisible;
            
            // If showing the panel, send the current value to the currency converter
            if (IsCurrencyPanelVisible)
            {
                SendToCurrency();
            }
        }

        /// <summary>
        /// Sends the current calculator result to the currency converter
        /// </summary>
        private void SendToCurrency()
        {
            try
            {
                if (!_calculator.HasError && decimal.TryParse(_calculator.CurrentInput, out decimal amount))
                {
                    _currencyVM.Amount = amount;
                }
            }
            catch (Exception)
            {
                // Ignore any errors
            }
        }

        /// <summary>
        /// Notifies that calculator properties have changed
        /// </summary>
        private void NotifyCalculatorPropertiesChanged()
        {
            OnPropertyChanged(nameof(CurrentInput));
            OnPropertyChanged(nameof(CurrentExpression));
            OnPropertyChanged(nameof(HasError));
            OnPropertyChanged(nameof(ErrorMessage));
        }

        #endregion
    }
}