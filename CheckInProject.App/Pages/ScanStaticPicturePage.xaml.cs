using CheckInProject.App.Utils;
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
        public IServiceProvider ServiceProvider;
        private IFaceDataManager FaceRecognitionAPI => ServiceProvider.GetRequiredService<IFaceDataManager>();
        private IPersonDatabaseManager DatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图片文件|*.jpg";
            if (dialog.ShowDialog() == true)
            {
                using (var targetFile = dialog.OpenFile())
                {
                    using (var targetBitmap = new Bitmap(targetFile))
                    {
                        var sourceImage = PictureConverters.ToBitmapImage(targetBitmap);
                        SourceImage = sourceImage;
                        var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFaceData(targetBitmap, null, null));
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
                                App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<MultipleResultsPage>());
                            }
                        }
                        if (string.IsNullOrEmpty(resultName)) resultName = "未识别到已知人脸";
                        ResultNames = resultName;
                    }
                }
            }
        }
        private async void CompareMultipleFaces_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图片文件|*.jpg";
            if (dialog.ShowDialog() == true)
            {
                using (var targetFile = dialog.OpenFile())
                {
                    using (var targetBitmap = new Bitmap(targetFile))
                    {
                        var sourceImage = PictureConverters.ToBitmapImage(targetBitmap);
                        SourceImage = sourceImage;
                        var targetFaceEncoding = await Task.Run(() => FaceRecognitionAPI.CreateFacesData(targetBitmap));
                        var knownFaces = await Task.Run(() => DatabaseAPI.GetFaceData().Select(t => t.ConvertToRawPersonDataBase()).ToList());
                        var result = await Task.Run(() => FaceRecognitionAPI.CompareFaces(knownFaces, targetFaceEncoding));
                        if (result.Count > 0)
                        {
                            var resultNameList = result.Select(t => t.Name).ToList();
                            var resultNameString = string.Join("/", resultNameList);
                            ResultNames = resultNameString;
                        }
                    }
                }
            }
        }
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

    }
}
