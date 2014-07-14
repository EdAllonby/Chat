using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace ChatClient.Views.Converter
{
    public class BitmapToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource bitmapSource = null;
            Bitmap bitmap = value as Bitmap;

            if (bitmap != null)
            {

                var hBitmap = bitmap.GetHbitmap();

                try
                {
                    bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
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
            BitmapSource bitmapSource = value as BitmapSource;

            if (bitmapSource != null)
            {
                Bitmap bitmap = new Bitmap(bitmapSource.PixelWidth,
                                           bitmapSource.PixelHeight,
                                           PixelFormat.Format32bppPArgb);

                BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                  ImageLockMode.WriteOnly,
                                                  PixelFormat.Format32bppPArgb);

                bitmapSource.CopyPixels(Int32Rect.Empty,
                                        data.Scan0,
                                        data.Height * data.Stride,
                                        data.Stride);

                bitmap.UnlockBits(data);

                return bitmap;
            }

            return null;
        }
    }
}