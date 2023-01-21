using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// DatabaseManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class DatabaseManagementPage : Page
    {
        private IPersonDatabaseManager FaceDatabaseAPI => ServiceProvider.GetRequiredService<IPersonDatabaseManager>();

        private ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();

        private IServiceProvider ServiceProvider;

        public DatabaseManagementPage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            ServiceProvider = serviceProvider;
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
    }
}
