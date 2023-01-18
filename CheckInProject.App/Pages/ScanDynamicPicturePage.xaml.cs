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
using Rect = OpenCvSharp.Rect;
using CheckInProject.PersonDataCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CheckInProject.PersonDataCore.Models;
using System.Collections.Generic;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// ScanDynamicPicturePage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanDynamicPicturePage : Page, INotifyPropertyChanged
    {
        private IServiceProvider ServiceProvider;
        private IFaceDataManager FaceRecognitionAPI => ServiceProvider.GetRequiredService<IFaceDataManager>();
        private IPersonDatabaseManager DatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();
        private List<RawPersonDataBase> ResultItems => ServiceProvider.GetRequiredService<List<RawPersonDataBase>>();
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
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        public string ResultNames
        {
            get => _resultName;
            set
            {
                _resultName = value;
                NotifyPropertyChanged();
            }
        }
        private string _resultName = string.Empty;

        private void StartCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMode = !CameraMode;
            if (CameraMode) 
            {
                Task.Run(CaptureVideo);
            }
        }
        public async void CaptureVideo()
        {
            using (var capture = new VideoCapture(0))
            {
                var capturedTime = 0;
                if (!capture.IsOpened())
                    return;
                var image = new Mat();
                CascadeClassifier cascade = new CascadeClassifier("haarcascade_frontalface_alt.xml");
                while (CameraMode && capturedTime < 15)
                {
                    capture.Read(image);
                    if (image.Empty())
                        break;
                    Rect[] faces = cascade.DetectMultiScale(
                        image: image,
                        scaleFactor: 1.1,
                        minNeighbors: 1,
                        flags: HaarDetectionTypes.DoRoughSearch | HaarDetectionTypes.ScaleImage,
                        minSize: new OpenCvSharp.Size(30, 30)
                    );
                    if (faces.Length <= 0) //没识别到人脸
                    {
                        var resultPicture = PictureConverters.ToBitmapImage(image.ToBitmap());
                        SourceImage = resultPicture;
                        await Task.Delay(100);
                        capturedTime = 0;
                    }
                    else
                    {
                        var resultPicture = PictureConverters.ToBitmapImage(image.ToBitmap());
                        SourceImage = resultPicture;
                        await Task.Delay(100);
                        capturedTime++;
                    }
                }
                CameraMode = false;
                var targetBitmap = image.ToBitmap();
                    var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFaceData(targetBitmap, null , null));
                    var knownFaces = await Task.Run(() => DatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                    var result = await Task.Run(() => FaceRecognitionAPI.CompareFace(knownFaces, targetFaceEncoding));
                var resultName = string.Empty;
                if (result.Count > 0)
                {
                    if (result.Count == 1) resultName = result.First().Name;
                    else
                    {
                        ResultItems.Clear();
                        ResultItems.AddRange(result);
                        Dispatcher.Invoke(() => App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<MultipleResultsPage>()));
                    }
                }
                if (string.IsNullOrEmpty(resultName)) resultName = "未识别到已知人脸";
                ResultNames = resultName;
                image.Dispose();
            }
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
