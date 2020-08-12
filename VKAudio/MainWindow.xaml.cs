#nullable enable
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using VKAudio;
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
            (new TrackDbContext()).Database.EnsureCreated();
            CurrentContent.Content = new LoginControl();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        public void OpenMainPage_Func(VkApi? api)
        {
            Debug.Print("MainScreen");
            background = new VK.Background(api);
            CurrentContent.Content = new MainControl();
        }

        private void PlayOrPause_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentContent.Content is MainControl)
            {
                background.ContinueOrPauseMusic();
            }
        }
    }
}