using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StylishCalculator.Models
{
    /// <summary>
    /// Core calculator engine that handles all mathematical operations
    /// </summary>
    public class CalculatorEngine
    {
        #region Properties

        /// <summary>
        /// Current input displayed on the calculator
        /// </summary>
        public string CurrentInput { get; private set; } = "0";

        /// <summary>
        /// Current expression being built
        /// </summary>
        public string CurrentExpression { get; private set; } = "";

        /// <summary>
        /// Current calculation result
        /// </summary>
        public decimal CurrentResult { get; private set; } = 0;

        /// <summary>
        /// Indicates if the calculator is in an error state
        /// </summary>
        public bool HasError { get; private set; } = false;

        /// <summary>
        /// Error message if HasError is true
        /// </summary>
        public string ErrorMessage { get; private set; } = "";

        /// <summary>
        /// Indicates if the last action was pressing an operation button
        /// </summary>
        private bool _lastActionWasOperation = false;

        /// <summary>
        /// Indicates if the last action was pressing the equals button
        /// </summary>
        private bool _lastActionWasEquals = false;

        /// <summary>
        /// The last operation that was performed
        /// </summary>
        private string _lastOperation = "";

        /// <summary>
        /// The last right operand used in a calculation
        /// </summary>
        private decimal _lastRightOperand = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Appends a digit to the current input
        /// </summary>
        /// <param name="digit">The digit to append (0-9)</param>
        public void AppendDigit(string digit)
        {
            if (HasError)
            {
                Clear();
            }

            if (_lastActionWasOperation || _lastActionWasEquals)
            {
                CurrentInput = digit;
                _lastActionWasOperation = false;
                _lastActionWasEquals = false;
            }
            else
            {
                // If current input is just "0", replace it with the new digit
                if (CurrentInput == "0")
                {
                    CurrentInput = digit;
                }
                else
                {
                    CurrentInput += digit;
                }
            }
        }

        /// <summary>
        /// Appends a decimal point to the current input
        /// </summary>
        public void AppendDecimalPoint()
        {
            if (HasError)
            {
                Clear();
            }

            if (_lastActionWasOperation || _lastActionWasEquals)
            {
                CurrentInput = "0.";
                _lastActionWasOperation = false;
                _lastActionWasEquals = false;
            }
            else
            {
                // Only add decimal point if there isn't one already
                if (!CurrentInput.Contains("."))
                {
                    CurrentInput += ".";
                }
            }
        }

        /// <summary>
        /// Sets the operation to perform
        /// </summary>
        /// <param name="operation">The operation symbol (+, -, *, /)</param>
        public void SetOperation(string operation)
        {
            if (HasError)
            {
                Clear();
            }

            try
            {
                // If we already have an operation pending, calculate the result first
                if (!string.IsNullOrEmpty(CurrentExpression) && !_lastActionWasOperation)
                {
                    CalculateResult();
                }

                // Store the current input as the left operand and the operation
                decimal leftOperand = decimal.Parse(CurrentInput, CultureInfo.InvariantCulture);
                CurrentExpression = $"{leftOperand} {operation}";
                CurrentResult = leftOperand;
                _lastActionWasOperation = true;
                _lastActionWasEquals = false;
                _lastOperation = operation;
            }
            catch (Exception ex)
            {
                SetError($"Operation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates the result of the current expression
        /// </summary>
        public void CalculateResult()
        {
            if (HasError)
            {
                return;
            }

            try
            {
                // If there's no expression, just return the current input
                if (string.IsNullOrEmpty(CurrentExpression))
                {
                    CurrentResult = decimal.Parse(CurrentInput, CultureInfo.InvariantCulture);
                    return;
                }

                // If the last action was equals, repeat the last operation with the last right operand
                if (_lastActionWasEquals)
                {
                    ApplyOperation(_lastOperation, _lastRightOperand);
                    CurrentInput = CurrentResult.ToString(CultureInfo.InvariantCulture);
                    return;
                }

                // Parse the expression to get the left operand and operation
                string[] parts = CurrentExpression.Split(' ');
                if (parts.Length < 2)
                {
                    return;
                }

                decimal leftOperand = decimal.Parse(parts[0], CultureInfo.InvariantCulture);
                string operation = parts[1];

                // Parse the right operand from the current input
                decimal rightOperand = decimal.Parse(CurrentInput, CultureInfo.InvariantCulture);
                _lastRightOperand = rightOperand;

                // Apply the operation
                ApplyOperation(operation, rightOperand);

                // Update the display
                CurrentInput = CurrentResult.ToString(CultureInfo.InvariantCulture);
                CurrentExpression = "";
                _lastActionWasEquals = true;
                _lastActionWasOperation = false;
            }
            catch (Exception ex)
            {
                SetError($"Calculation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all calculator state
        /// </summary>
        public void Clear()
        {
            CurrentInput = "0";
            CurrentExpression = "";
            CurrentResult = 0;
            HasError = false;
            ErrorMessage = "";
            _lastActionWasOperation = false;
            _lastActionWasEquals = false;
            _lastOperation = "";
            _lastRightOperand = 0;
        }

        /// <summary>
        /// Clears only the current entry
        /// </summary>
        public void ClearEntry()
        {
            if (HasError)
            {
                Clear();
                return;
            }

            CurrentInput = "0";
            _lastActionWasOperation = false;
            _lastActionWasEquals = false;
        }

        /// <summary>
        /// Removes the last character from the current input
        /// </summary>
        public void Backspace()
        {
            if (HasError)
            {
                Clear();
                return;
            }

            if (_lastActionWasOperation || _lastActionWasEquals)
            {
                return;
            }

            if (CurrentInput.Length > 1)
            {
                CurrentInput = CurrentInput.Substring(0, CurrentInput.Length - 1);
            }
            else
            {
                CurrentInput = "0";
            }
        }

        /// <summary>
        /// Toggles the sign of the current input
        /// </summary>
        public void ToggleSign()
        {
            if (HasError)
            {
                Clear();
                return;
            }

            if (CurrentInput != "0")
            {
                if (CurrentInput.StartsWith("-"))
                {
                    CurrentInput = CurrentInput.Substring(1);
                }
                else
                {
                    CurrentInput = "-" + CurrentInput;
                }
            }
        }

        /// <summary>
        /// Calculates the percentage of the current input
        /// </summary>
        public void CalculatePercentage()
        {
            if (HasError)
            {
                Clear();
                return;
            }

            try
            {
                decimal value = decimal.Parse(CurrentInput, CultureInfo.InvariantCulture);
                
                // If we have an expression, calculate percentage of the left operand
                if (!string.IsNullOrEmpty(CurrentExpression))
                {
                    string[] parts = CurrentExpression.Split(' ');
                    if (parts.Length >= 1)
                    {
                        decimal leftOperand = decimal.Parse(parts[0], CultureInfo.InvariantCulture);
                        value = leftOperand * (value / 100);
                    }
                }
                else
                {
                    // Otherwise just divide by 100
                    value = value / 100;
                }
                
                CurrentInput = value.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                SetError($"Percentage error: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the specified operation to the current result and right operand
        /// </summary>
        /// <param name="operation">The operation to apply</param>
        /// <param name="rightOperand">The right operand</param>
        private void ApplyOperation(string operation, decimal rightOperand)
        {
            switch (operation)
            {
                case "+":
                    CurrentResult += rightOperand;
                    break;
                case "-":
                    CurrentResult -= rightOperand;
                    break;
                case "*":
                case "×":
                    CurrentResult *= rightOperand;
                    break;
                case "/":
                case "÷":
                    if (rightOperand == 0)
                    {
                        SetError("Division by zero");
                        return;
                    }
                    CurrentResult /= rightOperand;
                    break;
                default:
                    SetError($"Unknown operation: {operation}");
                    break;
            }
        }

        /// <summary>
        /// Sets the calculator to an error state with the specified message
        /// </summary>
        /// <param name="message">The error message</param>
        private void SetError(string message)
        {
            HasError = true;
            ErrorMessage = message;
            CurrentInput = "Error";
        }

        #endregion
    }
}