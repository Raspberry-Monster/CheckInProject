using CheckInProject.App.Models;
using CheckInProject.App.Utils;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Rect = OpenCvSharp.Rect;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// ScanDynamicPicturePage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanDynamicPicturePage : Page, INotifyPropertyChanged
    {
        private readonly IServiceProvider ServiceProvider;
        private IFaceDataManager FaceRecognitionAPI => ServiceProvider.GetRequiredService<IFaceDataManager>();
        private IPersonDatabaseManager PersonDatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();
        private ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
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

        public List<CameraDevice> CameraDevices
        {
            get => _cameraDevices;
            set
            {
                _cameraDevices = value;
                NotifyPropertyChanged();
            }
        }
        private List<CameraDevice> _cameraDevices = new List<CameraDevice>();

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

        public bool CameraMode
        {
            get => _cameraMode;
            set
            {
                _cameraMode = value;
                NotifyPropertyChanged();
            }
        }
        private bool _cameraMode = false;

        public bool KeepRecognizing
        {
            get => _keepRecognizing;
            set
            {
                _keepRecognizing = value;
                NotifyPropertyChanged();
            }
        }
        private bool _keepRecognizing = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ScanDynamicPicturePage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            CameraDevices = CameraDeviceEnumerator.EnumerateCameras();
            InitializeComponent();
        }
        private void StartCaptureSingleButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMode = true;
            if (CameraSelector.SelectedIndex != -1)
            {
                var selectedCamera = CameraSelector.SelectedIndex;
                Task.Run(() => CaptureVideo(selectedCamera, false));
            }
            else
            {
                MessageBox.Show("请选择需要调用的摄像头", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                CameraMode = false;
            }
        }
        private void StartCaptureMultipleButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMode = true;
            if (CameraSelector.SelectedIndex != -1)
            {
                var selectedCamera = CameraSelector.SelectedIndex;
                Task.Run(() => CaptureVideo(selectedCamera, true));
            }
            else
            {
                MessageBox.Show("请选择需要调用的摄像头", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                CameraMode = false;
            }
        }

        public async void CaptureVideo(int cameraIndex, bool isMultiplePersonMode)
        {
            try
            {
                ResultNames = "未检测到已知结果";
                using (var capture = new VideoCapture(cameraIndex, VideoCaptureAPIs.DSHOW))
                {
                    var capturedTime = 0;
                    if (!capture.IsOpened())
                    {
                        return;
                    }
                    capture.FrameWidth = 640;
                    capture.FrameHeight = 480;
                    var image = new Mat();
                    CascadeClassifier cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
                    while (CameraMode)
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
                        if (faces.Length <= 0)
                        {
                            var resultPicture = PictureConverters.ToBitmapImage(image.ToBitmap());
                            SourceImage = resultPicture;
                            capturedTime = 0;
                        }
                        else
                        {
                            var resultPicture = PictureConverters.ToBitmapImage(image.ToBitmap());
                            SourceImage = resultPicture;
                            capturedTime++;
                            if (capturedTime >= 20)
                            {
                                var targetBitmap = image.ToBitmap();
                                if (isMultiplePersonMode)
                                {
                                    var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFacesData(targetBitmap));
                                    var knownFaces = await Task.Run(() => PersonDatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                                    var result = await Task.Run(() => FaceRecognitionAPI.CompareFaces(knownFaces, targetFaceEncoding));
                                    if (result.Count > 0)
                                    {
                                        if (result.Count == 1)
                                        {
                                            ResultNames = ResultItems.FirstOrDefault()?.Name ?? string.Empty;
                                            await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), result.First().StudentID);
                                            await Task.Delay(1000);
                                            if (!KeepRecognizing) break;
                                        }
                                        else
                                        {
                                            var resultNameList = result.Select(t => t.Name).ToList();
                                            ResultNames = string.Join("/", resultNameList);
                                            result.Select(t => t.StudentID).ToList().ForEach(async t => await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), t));
                                            if (!KeepRecognizing) break;
                                        }
                                    }
                                    capturedTime = 0;
                                }
                                else
                                {
                                    var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFaceData(targetBitmap, null, null));
                                    var knownFaces = await Task.Run(() => PersonDatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                                    var result = await Task.Run(() => FaceRecognitionAPI.CompareFace(knownFaces, targetFaceEncoding));
                                    if (result.Count > 0)
                                    {
                                        if (result.Count == 1)
                                        {
                                            ResultNames = result.First()?.Name ?? string.Empty;
                                            await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), result.First().StudentID);
                                            await Task.Delay(1000);
                                            if (!KeepRecognizing) Dispatcher.Invoke(() => App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>()));
                                            if (!KeepRecognizing) break;
                                        }
                                        else
                                        {
                                            ResultItems.Clear();
                                            ResultItems.AddRange(result);
                                            Dispatcher.Invoke(() => App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<MultipleResultsPage>()));
                                            ResultNames = "多个可能的检测结果";
                                            break;
                                        }
                                    }
                                    capturedTime = 0;
                                }
                            }
                        }
                    }
                    CameraMode = false;
                    image.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private void CancelCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            CameraMode = false;
        }
    }
}
