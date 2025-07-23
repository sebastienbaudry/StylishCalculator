using System;
using System.Windows;
using System.Windows.Input;
using StylishCalculator.ViewModels;

namespace StylishCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Enable keyboard input handling
            this.KeyDown += MainWindow_KeyDown;
            this.Focusable = true;
            this.Focus();
        }

        /// <summary>
        /// Handles the mouse left button down event on the title bar to enable window dragging
        /// </summary>
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double-click on title bar - toggle maximize/restore
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else
                {
                    WindowState = WindowState.Maximized;
                }
            }
            else
            {
                // Single click - begin dragging the window
                DragMove();
            }
        }

        /// <summary>
        /// Handles the click event on the minimize button
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the click event on the close button
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles keyboard input for calculator operations
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Get the MainViewModel from the CalculatorView's DataContext
            var calculatorView = FindName("CalculatorView") as FrameworkElement;
            if (calculatorView?.DataContext is MainViewModel viewModel)
            {
                HandleKeyboardInput(viewModel, e);
            }
        }

        /// <summary>
        /// Processes keyboard input and executes corresponding calculator commands
        /// </summary>
        private void HandleKeyboardInput(MainViewModel viewModel, KeyEventArgs e)
        {
            string keyString = GetKeyString(e.Key);
            bool shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            
            if (!string.IsNullOrEmpty(keyString))
            {
                bool handled = viewModel.HandleKeyboardInput(keyString, shiftPressed);
                if (handled)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Converts a Key enum value to a string representation
        /// </summary>
        private string GetKeyString(Key key)
        {
            switch (key)
            {
                // Digits
                case Key.D0:
                case Key.NumPad0:
                    return "0";
                case Key.D1:
                case Key.NumPad1:
                    return "1";
                case Key.D2:
                case Key.NumPad2:
                    return "2";
                case Key.D3:
                case Key.NumPad3:
                    return "3";
                case Key.D4:
                case Key.NumPad4:
                    return "4";
                case Key.D5:
                case Key.NumPad5:
                    return "5";
                case Key.D6:
                case Key.NumPad6:
                    return "6";
                case Key.D7:
                case Key.NumPad7:
                    return "7";
                case Key.D8:
                case Key.NumPad8:
                    return "8";
                case Key.D9:
                case Key.NumPad9:
                    return "9";

                // Operations
                case Key.Add:
                    return "+";
                case Key.OemPlus:
                    return Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? "+" : "";
                case Key.Subtract:
                case Key.OemMinus:
                    return "-";
                case Key.Multiply:
                    return "*";
                case Key.Divide:
                    return "/";
                case Key.OemQuestion:
                    return Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? "/" : "";

                // Special keys
                case Key.Decimal:
                case Key.OemPeriod:
                    return ".";
                case Key.Enter:
                    return "enter";
                case Key.Escape:
                    return "escape";
                case Key.Delete:
                    return "delete";
                case Key.Back:
                    return "backspace";
                case Key.Space:
                    return "space";

                default:
                    return "";
            }
        }

        /// <summary>
        /// Handles the resize grip mouse down event to enable window resizing
        /// </summary>
        private void ResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                // Begin resizing the window from the bottom-right corner
                var hwndSource = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
                if (hwndSource != null)
                {
                    System.Windows.Interop.HwndSource.FromHwnd(hwndSource.Handle).CompositionTarget.TransformToDevice.Transform(new Point(this.ActualWidth, this.ActualHeight));
                }
                
                // Use Windows API to resize
                ResizeWindow();
            }
        }

        /// <summary>
        /// Initiates window resizing using Windows API
        /// </summary>
        private void ResizeWindow()
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            const int WM_SYSCOMMAND = 0x112;
            const int SC_SIZE = 0xF000;
            const int WMSZ_BOTTOMRIGHT = 8;
            
            System.Windows.Interop.HwndSource.FromHwnd(hwnd)?.AddHook(WindowProc);
            NativeMethods.SendMessage(hwnd, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + WMSZ_BOTTOMRIGHT), IntPtr.Zero);
        }

        /// <summary>
        /// Window procedure for handling resize messages
        /// </summary>
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// Native methods for window resizing
        /// </summary>
        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        }
    }
}