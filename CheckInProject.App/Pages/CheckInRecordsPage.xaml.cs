using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// CheckInRecordsPage.xaml 的交互逻辑
    /// </summary>
    public partial class CheckInRecordsPage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public List<CheckInDataExportModels> ListBoxItems => CheckInManager.QueryTodayRecords();
        public CheckInRecordsPage(IServiceProvider provider)
        {
            ServiceProvider = provider;
            InitializeComponent();
        }

        private async void ExportCheckInDataToExcelFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var fileSaveDialog = new SaveFileDialog();
                fileSaveDialog.Filter = "Excel文件|.xlsx";
                if (fileSaveDialog.ShowDialog() == true)
                {
                    await CheckInManager.ExportRecordsToExcelFile(ExportTypeEnum.CheckedIn, fileSaveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}