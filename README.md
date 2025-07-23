# Stylish Calculator

A modern Windows desktop calculator application with a stylish dark mode UI featuring neon accents and glass-like transparency effects.

## Features

- Basic arithmetic operations (addition, subtraction, multiplication, division)
- Currency conversion functionality
- Dark mode UI with neon accents
- Glass-like transparency effects
- Smooth animations and visual feedback
- Error handling and input validation

## Screenshots

*Screenshots will be added after the application is built and running.*

## Requirements

- Windows 10 or later
- .NET 6.0 or later

## Building and Running

1. Clone the repository
2. Open the solution in Visual Studio 2019/2022
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

Alternatively, you can build and run from the command line:

```
dotnet build
dotnet run
```

## Architecture

The application is built using the MVVM (Model-View-ViewModel) pattern:

- **Models**: Contains the core calculation logic and currency conversion functionality
- **ViewModels**: Connects the UI to the models and handles user interactions
- **Views**: The XAML UI with dark mode, neon accents, and glass effects

## Project Structure

```
StylishCalculator/
├── App.xaml                  # Application entry point
├── MainWindow.xaml           # Main application window
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
├── Converters/
│   └── BooleanToVisibilityConverter.cs  # XAML value converter
└── Resources/
    └── calculator.ico        # Application icon
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.