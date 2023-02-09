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
                    var image = new Mat();
                    while (CameraMode)
                    {
                        capture.Read(image);
                        if (image.Empty())
                            break;
                        var faceCount = FaceRecognitionAPI.GetFaceCount(image);
                        var resultPicture = PictureConverters.ToBitmapImage(faceCount.RetangleImage);
                        SourceImage = resultPicture;
                        if (faceCount.Count <= 0)
                        {
                            capturedTime = 0;
                        }
                        else
                        {
                            capturedTime++;
                            if (capturedTime >= 20)
                            {
                                var targetBitmap = image.ToBitmap();
                                if (isMultiplePersonMode)
                                {
                                    var targetFaceBitmapModels = await Task.Run(() => FaceRecognitionAPI.GetFacesImage(targetBitmap));
                                    var targetFaceBitmapList = targetFaceBitmapModels.FaceImages;
                                    var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFacesData(targetFaceBitmapList));
                                    var knownFaces = await Task.Run(() => PersonDatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                                    var result = await Task.Run(() => FaceRecognitionAPI.CompareFaces(knownFaces, targetFaceEncoding));
                                    if (result.Count > 0)
                                    {
                                        if (result.Count == 1)
                                        {
                                            ResultNames = result.FirstOrDefault()?.Name ?? string.Empty;
                                            await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), result.First().StudentID);
                                            await Task.Delay(1000);
                                            if (!KeepRecognizing) Dispatcher.Invoke(() => App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>()));
                                            if (!KeepRecognizing) break;
                                        }
                                        else
                                        {
                                            var resultNameList = result.Select(t => t.Name).ToList();
                                            ResultNames = string.Join("/", resultNameList);
                                            result.Select(t => t.StudentID).ToList().ForEach(async t => await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), t));
                                            await Task.Delay(1000);
                                            if (!KeepRecognizing) Dispatcher.Invoke(() => App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>()));
                                            if (!KeepRecognizing) break;
                                        }
                                    }
                                    capturedTime = 0;
                                }
                                else
                                {
                                    var targetFaceBitmapModels = await Task.Run(() => FaceRecognitionAPI.GetFaceImage(targetBitmap));
                                    var targetFaceBitmap = targetFaceBitmapModels.FaceImages.First();
                                    var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFaceData(targetFaceBitmap, null, null));
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
