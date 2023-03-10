using CheckInProject.App.Utils;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// ScanStaticPicturePage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanStaticPicturePage : Page, INotifyPropertyChanged
    {
        private readonly IServiceProvider ServiceProvider;
        private IFaceDataManager FaceRecognitionAPI => ServiceProvider.GetRequiredService<IFaceDataManager>();
        private IPersonDatabaseManager DatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();
        private ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        private List<RawPersonDataBase> ResultItems => ServiceProvider.GetRequiredService<List<RawPersonDataBase>>();

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

        public event PropertyChangedEventHandler? PropertyChanged;

        public ScanStaticPicturePage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void CompareSingleFace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = $"图片文件|{PictureConverters.SupportedPictureExtensions}";
                if (dialog.ShowDialog() == true)
                {
                    using (var targetFile = dialog.OpenFile())
                    {
                        using (var sourceBitmap = new Bitmap(targetFile))
                        {
                            var targetFaceBitmapModels = await Task.Run(() => FaceRecognitionAPI.GetFaceImage(sourceBitmap));
                            using (var displayFaceBitmap =  targetFaceBitmapModels.RetangleImage)
                            {
                                var targetFaceBitmap = targetFaceBitmapModels.FaceImages.First();
                                SourceImage = await Task.Run(() => PictureConverters.ToBitmapImage(displayFaceBitmap));
                                var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFaceData(targetFaceBitmap, null, null));
                                var knownFaces = await Task.Run(() => DatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                                var result = await Task.Run(() => FaceRecognitionAPI.CompareFace(knownFaces, targetFaceEncoding));
                                var resultName = string.Empty;
                                if (result.Count > 0)
                                {
                                    if (result.Count == 1)
                                    {
                                        resultName = result.First().Name;
                                        if (string.IsNullOrEmpty(resultName)) resultName = "未识别到已知人脸";
                                        ResultNames = resultName ?? string.Empty;
                                        await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), result.First().StudentID);
                                        await Task.Delay(1000);
                                        App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>());

                                    }
                                    else
                                    {
                                        ResultItems.Clear();
                                        ResultItems.AddRange(result);
                                        App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<MultipleResultsPage>());
                                        resultName = "多个检测结果";
                                    }
                                }
                                else
                                {
                                    ResultNames = "未识别到已知人脸";
                                }
                                targetFaceBitmap?.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void CompareMultipleFaces_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = $"图片文件|{PictureConverters.SupportedPictureExtensions}";
                if (dialog.ShowDialog() == true)
                {
                    using (var targetFile = dialog.OpenFile())
                    {
                        using (var sourceBitmap = new Bitmap(targetFile))
                        {
                            var resultNameString = string.Empty;
                            var targetFacesBitmapModels = await Task.Run(() => FaceRecognitionAPI.GetFacesImage(sourceBitmap));
                            using (var displayFaceBitmap = targetFacesBitmapModels.RetangleImage)
                            {
                                var targetFacesBitmapList = targetFacesBitmapModels.FaceImages;
                                SourceImage = await Task.Run(() => PictureConverters.ToBitmapImage(displayFaceBitmap));
                                var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFacesData(targetFacesBitmapList));
                                var knownFaces = await Task.Run(() => DatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                                var result = await Task.Run(() => FaceRecognitionAPI.CompareFaces(knownFaces, targetFaceEncoding));
                                if (result.Count > 0)
                                {
                                    var resultNameList = result.Select(t => t.Name).ToList();
                                    resultNameString = string.Join("/", resultNameList);
                                    result.Select(t => t.StudentID).ToList().ForEach(async t => await CheckInManager.CheckIn(DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), t));
                                    ResultNames = resultNameString;
                                    await Task.Delay(1000);
                                    App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>());
                                }
                                else
                                {
                                    resultNameString = "未识别到已知人脸";
                                    ResultNames = resultNameString;
                                }
                                foreach (var item in targetFacesBitmapList)
                                {
                                    item?.Dispose();
                                }
                            }
                        }
                    }
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

    }
}
