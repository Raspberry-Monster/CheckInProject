using CheckInProject.App.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            ServiceProvider = provider;
            InitializeComponent();
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
