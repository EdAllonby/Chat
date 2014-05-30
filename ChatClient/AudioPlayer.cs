using System;
using System.IO;
using System.Media;

namespace ChatClient
{
    public sealed class AudioPlayer : IAudioPlayer
    {
        private readonly SoundPlayer soundPlayer = new SoundPlayer();

        public void Play(Stream resource)
        {
            using (resource)
            {
                soundPlayer.Stream = resource;
                soundPlayer.Play();
            }
        }

        public void Dispose()
        {
            soundPlayer.Dispose();
        }
    }
}