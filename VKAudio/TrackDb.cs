using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model.Attachments;

namespace VKAudio
{
    public class TrackDbContext : DbContext
    {
        public DbSet<OfflineTrack> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\ProgramData\\ramil2911\\VKAudio\\tracks.db");
        }
    }

    public class OfflineTrack
    {
        private Uri _url;
        [Key]
        public long? Id { get; private set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public int Duration { get; private set; }
        public bool? IsHq { get; private set; }
        
        public bool IsCached { get; private set; }

        public Uri Url
        {
            get => _url;
            private set => _url = value.DecodeAudioUrl();
        }

        public OfflineTrack() { }

        public OfflineTrack(Audio onlineTrack)
        {
            Id = onlineTrack.Id;
            Title = onlineTrack.Title;
            Artist = onlineTrack.Artist;
            Duration = onlineTrack.Duration;
            IsHq = onlineTrack.IsHq;
            Url = onlineTrack.Url;
        }

        public string Cache(VkApi api)
        {
            if (IsCached)
                return Directory.GetFiles($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{Id}", "*.mp3")[0];

            api.Audio.Download(Url, $"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{Id}");
            IsCached = true;
            return Directory.GetFiles($"C:\\ProgramData\\ramil2911\\VKAudio\\audiocache{Id}", "*.mp3")[0];
        }
        
        public static IEnumerable<OfflineTrack> ToOffline(IEnumerable<Audio> onlineTracks) 
            => onlineTracks.Select(onlineTrack => new OfflineTrack(onlineTrack));
        
    }
}