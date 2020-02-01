using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VKAudio;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model.Attachments;
using VkNet.Utils;

namespace VK
{
    public class Background
    {
        private VkNet.Model.Attachments.Audio currentTrack;
        private VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> myTracks;

        private System.Timers.Timer nextTrackTimer;
        private WaveOutEvent waveOut;

        private string[] files;

        private bool isPaused;

        private VkApi api;

        public Audio CurrentTrack { get => currentTrack; }
        public VkCollection<Audio> MyTracks { get => myTracks; set => myTracks = value; }
        public bool IsPaused { get => isPaused; }
        public VkApi Api { get => api; }

        public Background(VkApi myApi)
        {
            api = myApi;

            nextTrackTimer = new System.Timers.Timer();
            nextTrackTimer.Elapsed += OnNextTrack;
            nextTrackTimer.AutoReset = false;
        }

        public async Task PlayMusic(VkNet.Model.Attachments.Audio track)
        {
            await Task.Run(() =>
            {
                Debug.WriteLine($"Starting {track.Title}--{track.Id}");
                if (waveOut != null) { waveOut.Stop(); };
                if (Directory.Exists($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}"))
                {
                    Debug.WriteLine("Track already exists");
                    try
                    {
                        files = Directory.GetFiles($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}", "*.mp3");
                        Mp3FileReader reader = new Mp3FileReader(files[0]);
                        waveOut = new WaveOutEvent();
                        waveOut.Init(reader);
                        waveOut.Play();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine($"{track.Title} cannot be loaded");
                        Debug.WriteLine($"{ex.Message}");
                        Debug.WriteLine($"{ex.StackTrace}");
                        return; //TODO: обработчик исключений
                    }

                }
                else
                {
                    Debug.WriteLine("Loading track");
                    Uri downloadURL = StaticFunctions.DecodeAudioUrl(track.Url); //творим чудеса криптографии
                    Debug.WriteLine($"URL is {downloadURL}");
                    api.Audio.Download(downloadURL, $"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}");
                    files = Directory.GetFiles($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}", "*.mp3");
                    Mp3FileReader reader = new Mp3FileReader(files[0]);
                    waveOut = new WaveOutEvent();
                    waveOut.Init(reader);
                    waveOut.Play();
                }
                currentTrack = track;
            });

            isPaused = false;

            Debug.WriteLine("Starting timer");
            nextTrackTimer.Interval = track.Duration * 1000 + 1;
            nextTrackTimer.Start();
            Debug.WriteLine("Timer started");
        }

        public async Task PauseMusic()
        {
            await Task.Run(() =>
            {
                waveOut.Pause();
            });
            isPaused = true;
            nextTrackTimer.Stop();
        }

        public async Task ContinueMusic()
        {
            await Task.Run(() =>
            {
                waveOut.Play();
            });
            isPaused = false;
            nextTrackTimer.Start();
        }

        public async Task StopMusic()
        {
            await Task.Run(() =>
            {
                currentTrack = null;
                waveOut.Stop();
            });
            isPaused = true;
            nextTrackTimer.Stop();
        }

        public async Task UpdateAudioList()
        {
            await Task.Run(async () =>
            {
                myTracks = await api.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
                {
                    OwnerId = api.UserId,
                });
            });
        }

        private void OnNextTrack(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("Event: Next Track");
            int nextTrack = myTracks.IndexOf(currentTrack) + 1;
            if (myTracks.Count() < nextTrack + 1)
            {
                nextTrack = 0;
            }
            Debug.WriteLine($"Next is \"{myTracks[nextTrack].Title}\"");
            PlayMusic(myTracks[nextTrack]);
        }
    }
}