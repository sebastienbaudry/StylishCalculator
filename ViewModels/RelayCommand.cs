using System;
using System.Windows.Input;

namespace StylishCalculator.ViewModels
{
    /// <summary>
    /// A command implementation that relays its functionality to other objects by invoking delegates
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Event that is fired when the ability to execute the command changes
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new command that can always execute
        /// </summary>
        /// <param name="execute">The execution logic</param>
        public RelayCommand(Action<object?> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command
        /// </summary>
        /// <param name="execute">The execution logic</param>
        /// <param name="canExecute">The execution status logic</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute; // This can be null, which is fine
        }

        /// <summary>
        /// Determines whether this command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>True if this command can be executed; otherwise, false</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter!);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        public void Execute(object? parameter)
        {
            _execute(parameter!);
        }
    }
}