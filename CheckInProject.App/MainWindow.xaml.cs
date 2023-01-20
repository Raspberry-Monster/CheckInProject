using CheckInProject.App.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace CheckInProject.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider ServiceProvider;
        public MainWindow(IServiceProvider provider)
        {
            InitializeComponent();
            if (!File.Exists("PersonData.db") || !File.Exists("CheckInData.db"))
            {
                MessageBox.Show("程序文件损坏，请重新解压。", "灾难性故障", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }
            ServiceProvider = provider;
            App.RootFrame = RootFrame;
        }
        private void ScanStaticPicturePage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<ScanStaticPicturePage>());
        }

        private void ScanDynamicPicturePage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<ScanDynamicPicturePage>());
        }
        private void FaceDataManagementPage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<FaceDataManagementPage>());
        }

        private void CheckInRecordsPage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>());
        }

        private void UncheckedPeoplePage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<UncheckedPeoplePage>());
        }
        private void DatabaseManagementPage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<DatabaseManagementPage>());
        }
    }
}
