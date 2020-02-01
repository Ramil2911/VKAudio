using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using VKAudio;

namespace VK
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private int audioDelta = 0; //maximum ListView offset


        public MainControl()
        {
            InitializeComponent();

            DrawAudioList();
        }

        private async Task DrawAudioList()
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            await window.background.UpdateAudioList();
            int i = 0;
            while(i < 10)
            {
                try
                {
                    AudioList.Items.Add(new MusicListBoxControl(window.background.MyTracks[i]));
                    i++;
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }
                catch(IndexOutOfRangeException)
                {
                    break;
                }
            }
        }

        private new void PreviewMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ScrollBar.LineUpCommand.Execute(null, e.OriginalSource as IInputElement);
            }
            if (e.Delta < 0)
            {
                ScrollBar.LineDownCommand.Execute(null, e.OriginalSource as IInputElement);
            }
            e.Handled = true;
        }

        private void ScrollEvent(object sender, ScrollChangedEventArgs e)
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            try
            {
                if (e.VerticalOffset / 3 > audioDelta)
                {
                    AudioList.Items.Add(new MusicListBoxControl(window.background.MyTracks[AudioList.Items.Count]));
                    Debug.WriteLine($"Loaded {AudioList.Items.Count} items.");
                }
                if(e.VerticalOffset / 3 > audioDelta)
                {
                    audioDelta = Convert.ToInt32(e.VerticalOffset / 3);
                }
            }
            catch(IndexOutOfRangeException)
            {
                Debug.WriteLine("End of audiolist");
            }
            catch(ArgumentOutOfRangeException)
            {
                Debug.WriteLine("Argument is out of range");
            }

        }
    }
}