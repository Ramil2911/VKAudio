using System.Windows;
using VkNet;

namespace VK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Background background;

        public MainWindow()
        {
            InitializeComponent();

            CurrentContent.Content = new LoginControl();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        public void OpenMainPage_Func(VkApi api)
        {
            background = new VK.Background(api);
            CurrentContent.Content = new MainControl();
        }
    }
}