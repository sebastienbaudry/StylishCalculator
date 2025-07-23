using System;
using System.Windows;
using System.Windows.Input;

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
    }
}