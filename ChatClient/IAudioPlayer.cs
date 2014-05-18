using System.IO;

namespace ChatClient
{
    public interface IAudioPlayer
    {
        void Play(Stream resource);
    }
}