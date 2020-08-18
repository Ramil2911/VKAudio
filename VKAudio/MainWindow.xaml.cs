#nullable enable
using System.Diagnostics;
using System.IO;
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
            if (!Directory.Exists("C:\\ProgramData\\ramil2911\\VKAudio"))
                Directory.CreateDirectory("C:\\ProgramData\\ramil2911\\VKAudio");
            if (!File.Exists("C:\\ProgramData\\ramil2911\\VKAudio\\token"))
                File.Create("C:\\ProgramData\\ramil2911\\VKAudio\\token");
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