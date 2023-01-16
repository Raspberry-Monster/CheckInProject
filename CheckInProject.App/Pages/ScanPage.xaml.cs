using CheckInProject.Core.Interfaces;
using FaceRecognitionDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// ScanPage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanPage : Page
    {
        public IServiceProvider ServiceProvider;
        private IFaceDataManager FaceRecognitionAPI => ServiceProvider.GetRequiredService<IFaceDataManager>();
        private IDatabaseManager DatabaseAPI => ServiceProvider.GetRequiredService<IDatabaseManager>();
        public ScanPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图片文件|*.jpg";
            if (dialog.ShowDialog() == true)
            {
                using (var targetFile = dialog.OpenFile()) 
                {
                    using (var targetBitmap = new Bitmap(targetFile))
                    {
                        var targetFaceEncoding = FaceRecognitionAPI.CreateFaceData(targetBitmap,"");
                        var knownFaces = DatabaseAPI.GetFaceData().Select(t=>t.ConvertToRawFaceDataBase()).ToList();
                        var result = FaceRecognitionAPI.CompareFaces(knownFaces, targetFaceEncoding);
                        if (result.Count > 0)
                        {
                            MessageBox.Show(string.Join("/", result));
                        }
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        
    }
}
