using System;
using System.IO;
using System.Media;

namespace ChatClient
{
    public class AudioPlayer : IAudioPlayer
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
    }
}