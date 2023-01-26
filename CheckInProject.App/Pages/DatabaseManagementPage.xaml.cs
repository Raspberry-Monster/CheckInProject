using CheckInProject.App.Models;
using CheckInProject.App.Utils;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// DatabaseManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class DatabaseManagementPage : Page, INotifyPropertyChanged
    {
        private IPersonDatabaseManager FaceDatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();

        private ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();

        private Settings ApplicationSettings => ServiceProvider.GetRequiredService<Settings>();

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isLoggedIn = false;

        private int FailedCount = 0;

        public bool ForgotPasswordShowStatus
        {
            get => _forgotPasswordShowStatus;
            set
            {
                _forgotPasswordShowStatus = value;
                NotifyPropertyChanged();
            }
        }
        private bool _forgotPasswordShowStatus = false;

        private readonly IServiceProvider ServiceProvider;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DatabaseManagementPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void ClearFaceDataDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var messageBoxResult = MessageBox.Show("您确定要清空人脸数据库吗? 此操作不可恢复", "信息", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    await FaceDatabaseAPI.ClearFaceData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ClearCheckInDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var messageBoxResult = MessageBox.Show("您确定要清空打卡数据库吗? 此操作不可恢复", "信息", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    await CheckInManager.ClearCheckInRecords();
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

        private void CheckPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(DatabaseCheckInPasswordBox.Password))
                {
                    MessageBox.Show("请输入密码", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (++FailedCount >= 15 && App.DisableDatabaseProtection) ForgotPasswordShowStatus = true;
                    return;
                }
                var targetPassword = PasswordUtils.CreatePasswordMD5(DatabaseCheckInPasswordBox.Password);
                if (ApplicationSettings.PasswordMD5 == targetPassword)
                {
                    IsLoggedIn = true;
                }
                else
                {
                    MessageBox.Show("密码错误, 请重新输入", "密码错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (++FailedCount >= 15 && App.DisableDatabaseProtection) ForgotPasswordShowStatus = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SetNewPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(NewPasswordBox.Password))
                {
                    MessageBox.Show("请输入密码", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                var targetPassword = PasswordUtils.CreatePasswordMD5(NewPasswordBox.Password);
                ApplicationSettings.PasswordMD5 = targetPassword;
                await ApplicationSettings.SaveSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<SetDatabasePasswordPage>());
        }

        private void OpenFaceDataManageMentPage_Click(object sender, RoutedEventArgs e)
        {
            App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<FaceDataManagementPage>());
        }
    }
}
