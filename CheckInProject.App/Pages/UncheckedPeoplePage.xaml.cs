using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CheckInProject.CheckInCore.Models;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Implementation;
using Microsoft.Win32;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// UncheckedPeoplePage.xaml 的交互逻辑
    /// </summary>
    public partial class UncheckedPeoplePage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public List<StringPersonDataBase> ListBoxItems => CheckInManager.QueryTodayUncheckedRecords();
        public UncheckedPeoplePage(IServiceProvider provider)
        {
            ServiceProvider = provider;
            InitializeComponent();
        }
        private void ExportCheckInDataToExcelFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var fileSaveDialog = new SaveFileDialog();
            fileSaveDialog.Filter = "Excel文件|.xlsx";
            if (fileSaveDialog.ShowDialog() == true)
            {
                CheckInManager.ExportRecordsToExcelFile(ExportTypeEnum.UncheckedIn, fileSaveDialog.FileName);
            }
        }
    }
}