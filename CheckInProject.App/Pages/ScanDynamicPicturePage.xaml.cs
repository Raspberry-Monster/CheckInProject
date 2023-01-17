using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CheckInProject.App.Utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// ScanDynamicPicturePage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanDynamicPicturePage : Page, INotifyPropertyChanged
    {
        public BitmapSource SourceImage
        {
            get => _sourceImage;
            set
            {
                _sourceImage = value;
                NotifyPropertyChanged();
            }
        }
        private BitmapSource _sourceImage = new BitmapImage();

        private bool CameraMode = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ScanDynamicPicturePage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
        }

        private void StartCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMode = !CameraMode;
            if (CameraMode) 
            {
                Task.Run(CaptureVideo);
            }
        }
        public void CaptureVideo()
        {
            var capture = new VideoCapture(0);
            if (!capture.IsOpened())
                return;
            var image = new Mat();
            while (CameraMode)
            {
                capture.Read(image);
                if (image.Empty())
                    break;
                var resultPicture = PictureConverters.ToBitmapImage(image.ToBitmap());
                SourceImage = resultPicture;
            }
            capture.Dispose();
        }
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
}
