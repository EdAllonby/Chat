using System.IO;

namespace ChatClient.ViewModels
{
    public interface IAudioPlayer
    {
        void Play(Stream resource);
    }
}