using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace CheckInProject.App.Utils
{
    public static class PictureConverters
    {
        public static BitmapImage ToBitmapImage(Bitmap bmp)
        {
            Stream pictureStream = new MemoryStream();
            bmp.Save(pictureStream,ImageFormat.Png);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = pictureStream; 
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
    }
}
