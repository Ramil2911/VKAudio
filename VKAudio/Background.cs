using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;
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

        private WaveOutEvent waveOut;

        private string[] files;

        private bool isPaused;

        public Audio CurrentTrack { get => currentTrack; }
        public VkCollection<Audio> MyTracks { get => myTracks; }
        public bool IsPaused { get => isPaused; }

        public async Task PlayMusic(VkNet.Model.Attachments.Audio track, VkApi api)
        {
            if (!isPaused)
            {
                await Task.Run(() =>
                {
                    if (waveOut != null) { waveOut.Stop(); };
                    if (Directory.Exists($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}"))
                    {
                        try
                        {
                            files = Directory.GetFiles($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}", "*.mp3");
                            Mp3FileReader reader = new Mp3FileReader(files[0]);
                            waveOut = new WaveOutEvent();
                            waveOut.Init(reader);
                            waveOut.Play();
                        }
                        catch
                        {
                            return; //TODO: обработчик исключений
                        }
                    }
                    else
                    {
                        Uri downloadURL = Decoder.DecodeAudioUrl(track.Url); //творим чудеса криптографии
                        api.Audio.Download(downloadURL, $"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}");
                        files = Directory.GetFiles($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{track.Id}", "*.mp3");
                        Mp3FileReader reader = new Mp3FileReader(files[0]);
                        waveOut = new WaveOutEvent();
                        waveOut.Init(reader);
                        waveOut.Play();
                    }
                    currentTrack = track;
                });
            }
            else
            {
                waveOut.Play();
            }

            isPaused = false;
        }

        public async Task PauseMusic()
        {
            await Task.Run(() =>
            {
                waveOut.Pause();
            });
            isPaused = true;
        }

        public async Task ContinueMusic()
        {
            await Task.Run(() =>
            {
                waveOut.Play();
            });
            isPaused = false;
        }

        public async Task StopMusic()
        {
            await Task.Run(() =>
            {
                currentTrack = null;
                waveOut.Stop();
            });
            isPaused = true;
        }

        public async Task UpdateAudioList(VkApi api)
        {
            await Task.Run(async () =>
            {
                myTracks = await api.Audio.GetAsync(new VkNet.Model.RequestParams.AudioGetParams()
                {
                    OwnerId = api.UserId,
                });
            });
        }
    }
}