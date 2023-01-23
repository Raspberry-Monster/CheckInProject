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
        public static Bitmap CompressImage(Bitmap sourceImage,int targetImageWidth, int targetImageHeight)
        {
            int sorceImageWidth = sourceImage.Width;
            int sorceImageHeight = sourceImage.Height;
            Bitmap targetImage = sourceImage;
            if (sorceImageWidth > sorceImageHeight)
            {
                if (sorceImageWidth > targetImageWidth)
                {
                    var tempHeight = sorceImageHeight / (sorceImageWidth / targetImageWidth); 
                    targetImage = new Bitmap(sourceImage, targetImageWidth, tempHeight);
                    return targetImage;
                }
            }
            else
            {
                if (sorceImageHeight > targetImageHeight)
                {
                    int tempWidth = sorceImageWidth / (sorceImageHeight / targetImageHeight);
                    targetImage = new Bitmap(sourceImage, tempWidth, targetImageHeight);
                    return targetImage;
                }
            }
            return targetImage;
        }
    }
}
