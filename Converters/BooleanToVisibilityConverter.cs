using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StylishCalculator.Converters
{
    /// <summary>
    /// Converts a boolean value to a Visibility value
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Visibility value
        /// </summary>
        /// <param name="value">The boolean value to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">Optional parameter (if "Invert" or "Reverse", the conversion is reversed)</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>Visibility.Visible if true, Visibility.Collapsed if false (or vice versa if parameter is "Invert" or "Reverse")</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;
            bool invert = parameter is string s && (s == "Invert" || s == "Reverse");
            
            if (invert)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a Visibility value back to a boolean value
        /// </summary>
        /// <param name="value">The Visibility value to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">Optional parameter (if "Invert" or "Reverse", the conversion is reversed)</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>true if Visible, false if Collapsed (or vice versa if parameter is "Invert" or "Reverse")</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = value is Visibility v ? v : Visibility.Collapsed;
            bool invert = parameter is string s && (s == "Invert" || s == "Reverse");
            
            if (invert)
            {
                return visibility != Visibility.Visible;
            }
            
            return visibility == Visibility.Visible;
        }
    }
}