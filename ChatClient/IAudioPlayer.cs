using System;
using System.IO;

namespace ChatClient
{
    public interface IAudioPlayer: IDisposable 
    {
        void Play(Stream resource);
    }
}