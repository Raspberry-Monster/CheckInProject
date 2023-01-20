using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Implementation;
using CheckInProject.PersonDataCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// FaceDataManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class FaceDataManagementPage : Page, INotifyPropertyChanged
    {
        private IServiceProvider ServiceProvider;

        public event PropertyChangedEventHandler? PropertyChanged;
        public string CurrentName
        {
            get => _currentName;
            set
            {
                _currentName = value;
                NotifyPropertyChanged();
            }
        }
        private string _currentName = string.Empty;
        public string PathName
        {
            get => _pathName;
            set
            {
                _pathName = value;
                NotifyPropertyChanged();
            }
        }
        private string _pathName = string.Empty;

        private IFaceDataManager FaceRecognitionAPI => ServiceProvider.GetRequiredService<IFaceDataManager>();
        private IPersonDatabaseManager DatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();
        private ICheckInManager CheckInManager =>ServiceProvider.GetRequiredService<ICheckInManager>();
        public IList<StringPersonDataBase> ListBoxItems => DatabaseAPI.GetFaceData();
        public FaceDataManagementPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private async void CreateFaceDataButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var faceDataList = new List<RawPersonDataBase>();
                using (var sourcePathSelector = new CommonOpenFileDialog())
                {
                    sourcePathSelector.IsFolderPicker = true;
                    if (sourcePathSelector.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var sourcePath = sourcePathSelector.FileName;
                        PathName = sourcePath;
                        uint index = 0;
                        var imageFiles = Directory.GetFiles(sourcePath);
                        foreach (var imageFile in imageFiles)
                        {
                            FileInfo fileInfo = new FileInfo(imageFile);
                            if (fileInfo.Extension == ".jpg" && ServiceProvider != null)
                            {
                                CurrentName = $"正在创建{fileInfo.Name}的人脸数据";
                                using (var imageBitmap = new Bitmap(imageFile))
                                {
                                    var sourceName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                                    var resultFaceData = await Task.Run(() => FaceRecognitionAPI.CreateFaceData(imageBitmap, sourceName, ++index));
                                    faceDataList.Add(resultFaceData);
                                }
                                CurrentName = $"已创建完成{fileInfo.Name}的人脸数据";
                            }
                        }
                        CurrentName = "正在向数据库导入数据";
                        await CheckInManager.ClearCheckInRecords();
                        var stringFaceDatas = faceDataList.Select(t => t.ConvertToStringPersonDataBase()).ToList();
                        await DatabaseAPI.ImportFaceData(stringFaceDatas);
                        CurrentName = "导入完成";
                        NotifyPropertyChanged(nameof(ListBoxItems));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
