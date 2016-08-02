using System.Collections.Generic;
using System.IO;

namespace ChatClient.ViewModels
{
    public static class FileStreamExtensions
    {
        public static bool IsJpegImage(this FileStream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            var jpegHeader = new List<byte> { 0xFF, 0xD8 };

            var header = new byte[2];

            fileStream.Read(header, 0, 2);

            fileStream.Seek(0, SeekOrigin.Begin);

            return header[0] == jpegHeader[0] &&
                   header[1] == jpegHeader[1];
        }

        public static bool IsPngImage(this FileStream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            var pngHeader = new List<byte> { 0x89, 0x50, 0x4E, 0x47 };

            var header = new byte[4];

            fileStream.Read(header, 0, 4);

            fileStream.Seek(0, SeekOrigin.Begin);

            return header[0] == pngHeader[0] &&
                   header[1] == pngHeader[1] &&
                   header[2] == pngHeader[2] &&
                   header[3] == pngHeader[3];
        }
    }
}