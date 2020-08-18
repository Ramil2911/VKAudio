using System.Windows;
using System.Windows.Threading;

namespace VK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Произошла неопознанная ошибка:\n {e.Exception}");

            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}