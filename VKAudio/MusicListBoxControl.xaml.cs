using System.Windows;
using System.Windows.Controls;
using VkNet;
using VkNet.Model.Attachments;

namespace VK
{
    /// <summary>
    /// Interaction logic for MusicListBoxControl.xaml
    /// </summary>
    public partial class MusicListBoxControl : UserControl
    {
        private VkNet.Model.Attachments.Audio track;

        public MusicListBoxControl(VkNet.Model.Attachments.Audio audio, VkApi myApi)
        {
            InitializeComponent();
            track = audio;

            TRACK_NAME.Text = track.Title.ToString();
            TRACK_AUTHOR.Text = track.Artist.ToString();
            if (track.IsHq ?? false)
            {
                IsHQ.Visibility = Visibility.Hidden;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            window.background.PlayMusic(track);
        }

        public Audio Track { get => track; set => track = value; }
    }
}