using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using NAudio.Wave;
using VKAudio;
using VkNet;
using VkNet.Model.RequestParams;

namespace VK
{
    public class Background
    {
        private readonly Timer _nextTrackTimer;
        private WaveOutEvent _waveOut;
        private VkApi Api { get; }
        private OfflineTrack CurrentTrack { get; set; }

        public Background(VkApi myApi)
        {
            Api = myApi;
            Debug.Print(Api.UserId.ToString());

            _nextTrackTimer = new Timer();
            _nextTrackTimer.Elapsed += OnNextTrack;
            _nextTrackTimer.AutoReset = false;
        }

        public async Task PlayMusic(OfflineTrack track)
        {
            await Task.Run(() =>
            {
                Debug.WriteLine($"Starting {track.Title}--{track.Id}");
                _waveOut?.Stop();
                if (track.IsCached)
                {
                    Debug.WriteLine("Track already exists");
                    try
                    {
                        var reader = new Mp3FileReader(track.Cache(Api));
                        _waveOut = new WaveOutEvent();
                        _waveOut.Init(reader);
                        _waveOut.Play();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine($"{track.Title} cannot be loaded");
                        Debug.WriteLine($"{ex.Message}");
                        Debug.WriteLine($"{ex.StackTrace}");
                        return; //TODO: обработчик исключений
                    }

                }
                else if(StaticFunctions.CheckForInternetConnection())
                {
                    var reader = new Mp3FileReader(track.Cache(Api));
                    _waveOut = new WaveOutEvent();
                    _waveOut.Init(reader);
                    _waveOut.Play();
                }
            });

            Debug.WriteLine("Starting timer");
            CurrentTrack = track;
            _nextTrackTimer.Interval = track.Duration * 1000 + 1;
            _nextTrackTimer.Start();
            Debug.WriteLine("Timer started");
        }

        public async Task ContinueOrPauseMusic()
        {
            if (_waveOut.PlaybackState == PlaybackState.Paused)
            {
                await Task.Run(() =>
                    _waveOut.Play());
                _nextTrackTimer.Start();
            }
            else
            {
                await Task.Run(() =>
                    _waveOut.Pause());
                _nextTrackTimer.Stop();
            }
        }

        public async Task UpdateAudioList()
        {
            await Task.Run(async () =>
            {
                await using var db = new TrackDbContext();
                if (StaticFunctions.CheckForInternetConnection())
                {
                    var tracks = await Api.Audio.GetAsync(new AudioGetParams
                    {
                        OwnerId = Api.Users.Get(new List<long>()).FirstOrDefault()?.Id,
                    });
                    foreach (var track in db.Tracks)
                    {
                        db.Tracks.Remove(track);
                    }
                    await db.Tracks.AddRangeAsync(OfflineTrack.ToOffline(tracks).ToList());
                    await db.SaveChangesAsync();
                }
            });
        }

        private void OnNextTrack(object sender, ElapsedEventArgs e)
        {
            using var db = new TrackDbContext();
            var tracks = db.Tracks.ToList();
            var nextTrack = tracks.FindIndex(x => x.Id == CurrentTrack.Id) + 1;
            if (db.Tracks.Count() < nextTrack + 1) nextTrack = 0;
            Debug.WriteLine($"Next is \"{tracks[nextTrack].Title}\"");
            PlayMusic(tracks[nextTrack]);
        }
    }
}