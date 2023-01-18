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
            InitializeComponent();
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

        private void ScanHistoryPage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<HistoryPage>());
        }
    }
}
