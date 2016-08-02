using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;

namespace ChatClient.ViewModels.Converter
{
    public class BitmapToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource bitmapSource = null;
            var bitmap = value as Bitmap;

            if (bitmap != null)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();

                try
                {
                    bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                catch (Win32Exception)
                {
                    bitmapSource = null;
                }
            }

            return bitmapSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmapSource = value as BitmapSource;

            if (bitmapSource != null)
            {
                var bitmap = new Bitmap(bitmapSource.PixelWidth,
                    bitmapSource.PixelHeight,
                    PixelFormat.Format32bppPArgb);

                BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppPArgb);

                bitmapSource.CopyPixels(Int32Rect.Empty,
                    data.Scan0,
                    data.Height*data.Stride,
                    data.Stride);

                bitmap.UnlockBits(data);

                return bitmap;
            }

            return null;
        }

        public ImageSource Convert(Bitmap bitmap)
        {
            return (ImageSource) Convert(bitmap, null, null, null);
        }
    }
}