using System;
using System.IO;
using System.Media;

namespace ChatClient.ViewModels
{
    public class AudioPlayer : IAudioPlayer, IDisposable
    {
        private readonly SoundPlayer player = new SoundPlayer();

        public void Play(Stream resource)
        {
            using (resource)
            {
                player.Stream = resource;
                player.Play();
            }
        }

        public void Dispose()
        {
            player.Dispose();
        }
    }
}