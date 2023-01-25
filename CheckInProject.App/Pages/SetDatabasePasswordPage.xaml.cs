using CheckInProject.App.Models;
using CheckInProject.App.Utils;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// SetDatabasePasswordPage.xaml 的交互逻辑
    /// </summary>
    public partial class SetDatabasePasswordPage : Page
    {
        private IServiceProvider ServiceProvider;
        private Settings ApplicationSettings => ServiceProvider.GetRequiredService<Settings>();
        private IPersonDatabaseManager FaceDatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();
        private ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public SetDatabasePasswordPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void SetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(PasswordBox.Password))
                {
                    var resultText = await Task.Run(() => PasswordUtils.CreatePasswordMD5(PasswordBox.Password));
                    ApplicationSettings.PasswordMD5 = resultText;
                    ApplicationSettings.IsFirstRun = false;
                    await FaceDatabaseAPI.ClearFaceData();
                    await CheckInManager.ClearCheckInRecords();
                    await ApplicationSettings.SaveSettings();
                    App.RootFrame?.Navigate(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
