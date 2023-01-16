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
        }
        private void ScanPage_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.Navigate(ServiceProvider.GetRequiredService<ScanPage>());
        }
    }
}
