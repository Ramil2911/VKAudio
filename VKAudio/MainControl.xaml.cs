using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VkNet;

namespace VK
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private VkApi api; //апи, хранит пользователя

        public MainControl(VkApi myApi)
        {
            InitializeComponent();

            api = myApi;

            _ = DrawAudioList();
        }

        private async Task DrawAudioList()
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            await window.background.UpdateAudioList(api);
            foreach (VkNet.Model.Attachments.Audio track in window.background.MyTracks)
            {
                AudioList.Items.Add(new MusicListBoxControl(track, ref api));
            }
        }
    }
}