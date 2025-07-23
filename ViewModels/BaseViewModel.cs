using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StylishCalculator.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels that implements INotifyPropertyChanged
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event that is fired when a property changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets a property value and raises the PropertyChanged event if the value has changed
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">Name of the property (automatically determined by the compiler)</param>
        /// <returns>True if the value was changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets a property value, raises the PropertyChanged event if the value has changed,
        /// and executes an action if specified
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">New value</param>
        /// <param name="action">Action to execute after the property is changed</param>
        /// <param name="propertyName">Name of the property (automatically determined by the compiler)</param>
        /// <returns>True if the value was changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T field, T value, Action? action, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            action?.Invoke();
            return true;
        }
    }
}