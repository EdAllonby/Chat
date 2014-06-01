using System;
using System.IO;

namespace ChatClient.ViewModels
{
    public interface IAudioPlayer : IDisposable
    {
        void Play(Stream resource);
    }
}