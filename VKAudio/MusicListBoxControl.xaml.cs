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

        private VkApi api;

        public MusicListBoxControl(VkNet.Model.Attachments.Audio audio, ref VkApi myApi)
        {
            InitializeComponent();
            track = audio;

            TRACK_NAME.Text = track.Title.ToString();
            TRACK_AUTHOR.Text = track.Artist.ToString();

            api = myApi;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            _ = window.background.PlayMusic(track, api);
        }

        public Audio Track { get => track; set => track = value; }
    }
}