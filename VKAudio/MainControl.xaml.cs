using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VK
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();

            _ = DrawAudioList();
        }

        private async Task DrawAudioList()
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            await window.background.UpdateAudioList();
            foreach (VkNet.Model.Attachments.Audio track in window.background.MyTracks)
            {
                AudioList.Items.Add(new MusicListBoxControl(track, window.background.Api));
            }
        }
    }
}