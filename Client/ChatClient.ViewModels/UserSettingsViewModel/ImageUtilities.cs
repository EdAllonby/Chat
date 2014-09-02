using System.Drawing;
using System.IO;

namespace ChatClient.ViewModels.UserSettingsViewModel
{
    internal static class ImageUtilities
    {
        public static bool TryLoadImageFromFile(string filename, out Image image)
        {
            image = null;

            using (FileStream fileStream = File.OpenRead(filename))
            {
                if (fileStream.IsJpegImage() || fileStream.IsPngImage())
                {
                    var bitmap = new Bitmap(fileStream);
                    var scaledBitmap = new Bitmap(bitmap, 300, 300);
                    image = scaledBitmap;
                    return true;
                }

                return false;
            }
        }
    }
}