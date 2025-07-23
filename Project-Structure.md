# Stylish Calculator - Project Structure

This document outlines the initial file structure and setup instructions for the Stylish Calculator WPF project.

## Initial Project Setup

1. Create a new WPF Application in Visual Studio
   - Project Name: `StylishCalculator`
   - Framework: `.NET Framework 4.7.2` or `.NET 6.0+`
   - Solution Name: `StylishCalculator`

2. Create the following folder structure in the project:
   ```
   StylishCalculator/
   ├── Models/
   ├── ViewModels/
   ├── Views/
   ├── Styles/
   ├── Services/
   ├── Helpers/
   └── Resources/
   ```

3. Add NuGet packages:
   - `Newtonsoft.Json` (for JSON parsing)
   - `Microsoft.Xaml.Behaviors.Wpf` (for advanced XAML behaviors)

## Initial Files to Create

### App.xaml
```xml
<Application x:Class="StylishCalculator.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:StylishCalculator"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/Styles/Colors.xaml"/>
                <ResourceDictionary Source="/Styles/Buttons.xaml"/>
                <ResourceDictionary Source="/Styles/TextBlocks.xaml"/>
                <ResourceDictionary Source="/Styles/Effects.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### MainWindow.xaml
```xml
<Window x:Class="StylishCalculator.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:StylishCalculator"
        xmlns:views="clr-namespace:StylishCalculator.Views"
        mc:Ignorable="d"
        Title="Stylish Calculator" 
        Height="600" 
        Width="400"
        MinHeight="500"
        MinWidth="350"
        Background="#121212"
        WindowStartupLocation="CenterScreen">
    <Grid>
        <views:CalculatorView/>
    </Grid>
</Window>
```

### Models/CalculatorEngine.cs
```csharp
using System;

namespace StylishCalculator.Models
{
    public class CalculatorEngine
    {
        // Properties
        public string CurrentInput { get; private set; } = "0";
        public string CurrentExpression { get; private set; } = "";
        public decimal CurrentResult { get; private set; } = 0;
        public bool HasError { get; private set; } = false;
        
        // Basic structure - to be implemented
        public void AppendDigit(string digit) { }
        public void AppendDecimalPoint() { }
        public void SetOperation(string operation) { }
        public void CalculateResult() { }
        public void Clear() { }
        public void ClearEntry() { }
        public void Backspace() { }
        public void ToggleSign() { }
        public void CalculatePercentage() { }
    }
}
```

### Models/CurrencyConverter.cs
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StylishCalculator.Models
{
    public class CurrencyConverter
    {
        // Properties
        public List<string> AvailableCurrencies { get; private set; } = new List<string>();
        public string FromCurrency { get; set; } = "USD";
        public string ToCurrency { get; set; } = "EUR";
        public decimal Amount { get; set; } = 1;
        public decimal ConvertedAmount { get; private set; } = 0;
        public DateTime LastUpdated { get; private set; }
        
        // Basic structure - to be implemented
        public async Task LoadCurrenciesAsync() { }
        public async Task ConvertAsync() { }
        public void SwapCurrencies() { }
    }
}
```

### ViewModels/BaseViewModel.cs
```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StylishCalculator.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
```

### ViewModels/MainViewModel.cs
```csharp
using System.Windows.Input;
using StylishCalculator.Models;

namespace StylishCalculator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Properties
        private CalculatorEngine _calculator = new CalculatorEngine();
        public CalculatorEngine Calculator => _calculator;

        private CurrencyViewModel _currencyVM = new CurrencyViewModel();
        public CurrencyViewModel CurrencyVM => _currencyVM;

        private bool _isCurrencyPanelVisible = false;
        public bool IsCurrencyPanelVisible
        {
            get => _isCurrencyPanelVisible;
            set => SetProperty(ref _isCurrencyPanelVisible, value);
        }
        
        // Commands - to be implemented
        public ICommand DigitCommand { get; private set; }
        public ICommand OperationCommand { get; private set; }
        public ICommand EqualsCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand BackspaceCommand { get; private set; }
        public ICommand ToggleSignCommand { get; private set; }
        public ICommand PercentageCommand { get; private set; }
        public ICommand ToggleCurrencyPanelCommand { get; private set; }
        
        public MainViewModel()
        {
            // Initialize commands
        }
    }
}
```

### ViewModels/CurrencyViewModel.cs
```csharp
using System.Windows.Input;
using StylishCalculator.Models;

namespace StylishCalculator.ViewModels
{
    public class CurrencyViewModel : BaseViewModel
    {
        private CurrencyConverter _converter = new CurrencyConverter();
        public CurrencyConverter Converter => _converter;
        
        // Commands - to be implemented
        public ICommand ConvertCommand { get; private set; }
        public ICommand SwapCurrenciesCommand { get; private set; }
        
        public CurrencyViewModel()
        {
            // Initialize commands
        }
    }
}
```

### Services/IExchangeRateService.cs
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StylishCalculator.Services
{
    public interface IExchangeRateService
    {
        Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency);
        Task<List<string>> GetAvailableCurrenciesAsync();
        DateTime LastUpdated { get; }
    }
}
```

### Services/ExchangeRateService.cs
```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StylishCalculator.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Dictionary<string, Dictionary<string, decimal>> _cachedRates = new Dictionary<string, Dictionary<string, decimal>>();
        
        public DateTime LastUpdated { get; private set; }
        
        // Basic structure - to be implemented
        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency) 
        {
            return new Dictionary<string, decimal>();
        }
        
        public async Task<List<string>> GetAvailableCurrenciesAsync() 
        {
            return new List<string> { "USD", "EUR", "GBP", "JPY", "CAD", "AUD", "CHF", "CNY" };
        }
    }
}
```

### Styles/Colors.xaml
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Base Colors -->
    <Color x:Key="BackgroundColor">#121212</Color>
    <Color x:Key="TextColor">#FFFFFF</Color>
    
    <!-- Neon Colors -->
    <Color x:Key="NeonCyan">#00FFFF</Color>
    <Color x:Key="NeonMagenta">#FF00FF</Color>
    <Color x:Key="NeonGreen">#00FF00</Color>
    <Color x:Key="NeonYellow">#FFFF00</Color>
    
    <!-- Brushes -->
    <SolidColorBrush x:Key="BackgroundBrush" Color="{StaticResource BackgroundColor}"/>
    <SolidColorBrush x:Key="TextBrush" Color="{StaticResource TextColor}"/>
    <SolidColorBrush x:Key="NeonCyanBrush" Color="{StaticResource NeonCyan}"/>
    <SolidColorBrush x:Key="NeonMagentaBrush" Color="{StaticResource NeonMagenta}"/>
    <SolidColorBrush x:Key="NeonGreenBrush" Color="{StaticResource NeonGreen}"/>
    <SolidColorBrush x:Key="NeonYellowBrush" Color="{StaticResource NeonYellow}"/>
    
</ResourceDictionary>
```

### Styles/Buttons.xaml
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Base Button Style -->
    <Style x:Key="GlassButtonStyle" TargetType="Button">
        <Setter Property="Background">
            <Setter.Value>
                <SolidColorBrush Color="#121212" Opacity="0.2"/>
            </Setter.Value>
        </Setter>
        <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="FontSize" Value="18"/>
        <Setter Property="Margin" Value="5"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="5">
                        <ContentPresenter HorizontalAlignment="Center" 
                                          VerticalAlignment="Center"/>
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
    
    <!-- Number Button Style -->
    <Style x:Key="NumberButtonStyle" TargetType="Button" BasedOn="{StaticResource GlassButtonStyle}">
        <Setter Property="BorderBrush" Value="{StaticResource NeonCyanBrush}"/>
    </Style>
    
    <!-- Operation Button Style -->
    <Style x:Key="OperationButtonStyle" TargetType="Button" BasedOn="{StaticResource GlassButtonStyle}">
        <Setter Property="BorderBrush" Value="{StaticResource NeonMagentaBrush}"/>
    </Style>
    
    <!-- Equals Button Style -->
    <Style x:Key="EqualsButtonStyle" TargetType="Button" BasedOn="{StaticResource GlassButtonStyle}">
        <Setter Property="BorderBrush" Value="{StaticResource NeonGreenBrush}"/>
    </Style>
    
    <!-- Function Button Style -->
    <Style x:Key="FunctionButtonStyle" TargetType="Button" BasedOn="{StaticResource GlassButtonStyle}">
        <Setter Property="BorderBrush" Value="{StaticResource NeonYellowBrush}"/>
    </Style>
    
</ResourceDictionary>
```

### Styles/TextBlocks.xaml
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Display TextBlock Style -->
    <Style x:Key="DisplayTextBlockStyle" TargetType="TextBlock">
        <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
        <Setter Property="FontSize" Value="36"/>
        <Setter Property="TextAlignment" Value="Right"/>
        <Setter Property="TextWrapping" Value="NoWrap"/>
        <Setter Property="Margin" Value="10"/>
        <Setter Property="FontFamily" Value="Consolas"/>
    </Style>
    
    <!-- Secondary Display TextBlock Style -->
    <Style x:Key="SecondaryDisplayTextBlockStyle" TargetType="TextBlock">
        <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
        <Setter Property="FontSize" Value="20"/>
        <Setter Property="TextAlignment" Value="Right"/>
        <Setter Property="TextWrapping" Value="NoWrap"/>
        <Setter Property="Margin" Value="10,0,10,10"/>
        <Setter Property="FontFamily" Value="Consolas"/>
        <Setter Property="Opacity" Value="0.7"/>
    </Style>
    
</ResourceDictionary>
```

### Styles/Effects.xaml
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Glow Effect -->
    <DropShadowEffect x:Key="CyanGlowEffect" 
                      Color="{StaticResource NeonCyan}" 
                      Direction="0" 
                      ShadowDepth="0" 
                      BlurRadius="10" 
                      Opacity="0.7"/>
    
    <DropShadowEffect x:Key="MagentaGlowEffect" 
                      Color="{StaticResource NeonMagenta}" 
                      Direction="0" 
                      ShadowDepth="0" 
                      BlurRadius="10" 
                      Opacity="0.7"/>
    
    <DropShadowEffect x:Key="GreenGlowEffect" 
                      Color="{StaticResource NeonGreen}" 
                      Direction="0" 
                      ShadowDepth="0" 
                      BlurRadius="10" 
                      Opacity="0.7"/>
    
    <DropShadowEffect x:Key="YellowGlowEffect" 
                      Color="{StaticResource NeonYellow}" 
                      Direction="0" 
                      ShadowDepth="0" 
                      BlurRadius="10" 
                      Opacity="0.7"/>
    
</ResourceDictionary>
```

### Views/CalculatorView.xaml
```xml
<UserControl x:Class="StylishCalculator.Views.CalculatorView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:StylishCalculator.Views"
             xmlns:vm="clr-namespace:StylishCalculator.ViewModels"
             mc:Ignorable="d" 
             d:DesignHeight="600" d:DesignWidth="400">
    
    <UserControl.DataContext>
        <vm:MainViewModel/>
    </UserControl.DataContext>
    
    <Grid Background="{StaticResource BackgroundBrush}">
        <!-- Basic structure - to be implemented -->
        <TextBlock Text="Stylish Calculator" 
                   Foreground="{StaticResource TextBrush}"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   FontSize="24"/>
    </Grid>
</UserControl>
```

### Views/CurrencyView.xaml
```xml
<UserControl x:Class="StylishCalculator.Views.CurrencyView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:local="clr-namespace:StylishCalculator.Views"
             xmlns:vm="clr-namespace:StylishCalculator.ViewModels"
             mc:Ignorable="d" 
             d:DesignHeight="200" d:DesignWidth="400">
    
    <UserControl.DataContext>
        <vm:CurrencyViewModel/>
    </UserControl.DataContext>
    
    <Grid Background="{StaticResource BackgroundBrush}">
        <!-- Basic structure - to be implemented -->
        <TextBlock Text="Currency Converter" 
                   Foreground="{StaticResource TextBrush}"
                   HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   FontSize="18"/>
    </Grid>
</UserControl>
```

## Next Steps

After setting up the initial project structure:

1. Implement the `CalculatorEngine` class with basic arithmetic operations
2. Design and implement the full calculator UI in `CalculatorView.xaml`
3. Implement the `MainViewModel` with command bindings
4. Add the glass effect and animations
5. Implement the currency conversion feature
6. Test and refine the application