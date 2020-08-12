using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace VKAudio
{
    public static class StaticFunctions
    {
        public static Uri DecodeAudioUrl(this Uri audioUrl)
        {
            var segments = audioUrl.Segments.ToList();

            segments.RemoveAt((segments.Count - 1) / 2);
            segments.RemoveAt(segments.Count - 1);

            segments[segments.Count - 1] = segments[segments.Count - 1].Replace("/", ".mp3");

            return new Uri($"{audioUrl.Scheme}://{audioUrl.Host}{string.Join("", segments)}{audioUrl.Query}");
        }
        
        public static bool CheckForInternetConnection()
        {
            try
            {
                using var client = new WebClient();
                using (client.OpenRead("http://google.com/generate_204")) 
                    return true;
            }
            catch
            {
                return false;
            }
        }
        
    }
    
    
}