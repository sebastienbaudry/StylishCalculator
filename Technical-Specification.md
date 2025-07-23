# Stylish Calculator - Technical Specification

## Overview

This document outlines the technical specifications for implementing a stylish Windows desktop calculator application using WPF. The calculator will feature basic arithmetic operations and currency conversion functionality, with a dark mode UI featuring neon accents and glass-like transparency effects.

## Technology Stack

- **Framework**: Windows Presentation Foundation (WPF)
- **Language**: C#
- **Target Platform**: Windows 10/11
- **Minimum .NET Version**: .NET Framework 4.7.2 or .NET 6.0+
- **IDE**: Visual Studio 2019/2022

## Project Structure

```
StylishCalculator/
├── App.xaml                  # Application entry point
├── App.xaml.cs               # Application code-behind
├── MainWindow.xaml           # Main application window
├── MainWindow.xaml.cs        # Main window code-behind
├── Models/
│   ├── CalculatorEngine.cs   # Core calculation logic
│   └── CurrencyConverter.cs  # Currency conversion logic
├── ViewModels/
│   ├── BaseViewModel.cs      # Base ViewModel with INotifyPropertyChanged
│   ├── MainViewModel.cs      # Main calculator ViewModel
│   └── CurrencyViewModel.cs  # Currency converter ViewModel
├── Views/
│   ├── CalculatorView.xaml   # Calculator UI
│   └── CurrencyView.xaml     # Currency converter UI
├── Styles/
│   ├── Colors.xaml           # Color definitions
│   ├── Buttons.xaml          # Button styles
│   ├── TextBlocks.xaml       # Text styles
│   └── Effects.xaml          # Visual effects
├── Services/
│   ├── IExchangeRateService.cs  # Exchange rate service interface
│   └── ExchangeRateService.cs   # Exchange rate service implementation
└── Helpers/
    ├── MathParser.cs         # Expression parsing helper
    └── AnimationHelper.cs    # Animation utility methods
```

## Core Components

### 1. Calculator Engine (Models/CalculatorEngine.cs)

The calculator engine will handle the mathematical operations and maintain the calculation state.

```csharp
public class CalculatorEngine
{
    // Properties
    public string CurrentInput { get; private set; }
    public string CurrentExpression { get; private set; }
    public decimal CurrentResult { get; private set; }
    public bool HasError { get; private set; }
    
    // Methods
    public void AppendDigit(string digit)
    public void AppendDecimalPoint()
    public void SetOperation(string operation)
    public void CalculateResult()
    public void Clear()
    public void ClearEntry()
    public void Backspace()
    public void ToggleSign()
    public void CalculatePercentage()
}
```

### 2. Currency Converter (Models/CurrencyConverter.cs)

The currency converter will handle fetching exchange rates and performing conversions.

```csharp
public class CurrencyConverter
{
    // Properties
    public List<string> AvailableCurrencies { get; private set; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
    public decimal Amount { get; set; }
    public decimal ConvertedAmount { get; private set; }
    
    // Methods
    public async Task LoadCurrenciesAsync()
    public async Task ConvertAsync()
    public void SwapCurrencies()
    public DateTime LastUpdated { get; private set; }
}
```

### 3. Exchange Rate Service (Services/ExchangeRateService.cs)

```csharp
public interface IExchangeRateService
{
    Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency);
    Task<List<string>> GetAvailableCurrenciesAsync();
    DateTime LastUpdated { get; }
}

public class ExchangeRateService : IExchangeRateService
{
    // Implementation using a free currency API
    // Will include caching mechanism to avoid excessive API calls
}
```

### 4. Main ViewModel (ViewModels/MainViewModel.cs)

```csharp
public class MainViewModel : BaseViewModel
{
    // Properties
    public CalculatorEngine Calculator { get; }
    public CurrencyViewModel CurrencyVM { get; }
    public bool IsCurrencyPanelVisible { get; set; }
    
    // Commands
    public ICommand DigitCommand { get; }
    public ICommand OperationCommand { get; }
    public ICommand EqualsCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand ToggleSignCommand { get; }
    public ICommand PercentageCommand { get; }
    public ICommand ToggleCurrencyPanelCommand { get; }
}
```

## UI Implementation

### Main Calculator UI

The main calculator UI will be implemented in XAML with the following key elements:

1. **Display Area**: TextBlock with right-aligned text for showing input and results
2. **Number Buttons**: 0-9 and decimal point with neon cyan styling
3. **Operation Buttons**: +, -, ×, ÷ with neon magenta styling
4. **Function Buttons**: Clear, Backspace, +/-, % with distinct styling
5. **Equals Button**: With neon green styling and prominent design
6. **Currency Toggle**: Button to show/hide the currency conversion panel

### Glass Effect Implementation

The glass effect will be implemented using a combination of:

1. **Opacity**: Setting appropriate opacity levels for controls
2. **Blur Effect**: Using the BlurEffect class in WPF
3. **Gradient Overlays**: Adding subtle gradient overlays to enhance the glass look

```xaml
<Style x:Key="GlassButtonStyle" TargetType="Button">
    <Setter Property="Background">
        <Setter.Value>
            <SolidColorBrush Color="#121212" Opacity="0.2"/>
        </Setter.Value>
    </Setter>
    <Setter Property="Effect">
        <Setter.Value>
            <BlurEffect Radius="5"/>
        </Setter.Value>
    </Setter>
    <!-- Additional styling properties -->
</Style>
```

### Animation Implementation

Animations will be implemented using WPF's animation framework:

1. **Button Press**: Using ScaleTransform and DoubleAnimation
2. **Number Input**: Using TranslateTransform for sliding effects
3. **Panel Transitions**: Using height animations for expanding/collapsing

```csharp
// Example animation helper method
public static void AnimateButtonPress(Button button)
{
    var scaleTransform = new ScaleTransform(1.0, 1.0);
    button.RenderTransform = scaleTransform;
    
    var animation = new DoubleAnimation
    {
        From = 1.0,
        To = 0.95,
        Duration = TimeSpan.FromMilliseconds(100),
        AutoReverse = true
    };
    
    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
}
```

## Currency Conversion Implementation

The currency conversion feature will:

1. Use a free exchange rate API (e.g., ExchangeRate-API)
2. Cache exchange rates to minimize API calls
3. Allow selection of base and target currencies
4. Display conversion results in real-time

```csharp
// Example API call implementation
private async Task<Dictionary<string, decimal>> FetchExchangeRatesAsync(string baseCurrency)
{
    using (var client = new HttpClient())
    {
        var response = await client.GetAsync($"https://api.exchangerate-api.com/v4/latest/{baseCurrency}");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        // Parse JSON response and extract rates
        // Implementation details will depend on the specific API used
    }
}
```

## Error Handling

The application will implement comprehensive error handling:

1. **Input Validation**: Prevent invalid inputs (e.g., multiple decimal points)
2. **Division by Zero**: Display appropriate error message
3. **API Failures**: Gracefully handle network issues or API limitations
4. **General Exceptions**: Catch and log unexpected errors

## Testing Strategy

The application should be tested for:

1. **Calculation Accuracy**: Verify all mathematical operations
2. **Currency Conversion**: Verify conversion accuracy
3. **UI Responsiveness**: Ensure smooth animations and transitions
4. **Error Handling**: Test all error scenarios
5. **Visual Appearance**: Verify styling across different Windows versions

## Packaging and Distribution

The application will be packaged as a standalone executable with all necessary dependencies included. Options include:

1. **ClickOnce Deployment**: For easy installation and updates
2. **MSIX Package**: For Windows Store distribution
3. **Standalone Executable**: For simple distribution

## Future Enhancements

Potential future enhancements could include:

1. **History Feature**: Track calculation history
2. **Additional Themes**: Allow user to select different color schemes
3. **Scientific Functions**: Add advanced mathematical functions
4. **Customizable Layout**: Allow users to rearrange buttons
5. **Keyboard Shortcuts**: Implement keyboard navigation