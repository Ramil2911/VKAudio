using System.Windows;
using System.Windows.Controls;
using VKAudio;
using VkNet;
using VkNet.Model.Attachments;

namespace VK
{
    /// <summary>
    /// Interaction logic for MusicListBoxControl.xaml
    /// </summary>
    public partial class MusicListBoxControl : UserControl
    {
        public MusicListBoxControl(OfflineTrack audio)
        {
            InitializeComponent();
            Track = audio;
            if (!Track.IsCached & !StaticFunctions.CheckForInternetConnection())
            {
                Play.IsEnabled = false;
            }
            TRACK_NAME.Text = Track.Title;
            TRACK_AUTHOR.Text = Track.Artist;
            if (Track.IsHq ?? false) IsHQ.Visibility = Visibility.Hidden;
            var trackDuration = Track.Duration % 60 < 10 ? $"{Track.Duration / 60}:0{Track.Duration % 60}" : $"{Track.Duration / 60}:{Track.Duration % 60}";
            TRACK_DURATION.Text = trackDuration;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            window?.background.PlayMusic(Track);
        }

        public OfflineTrack Track { get; set; }
    }
}