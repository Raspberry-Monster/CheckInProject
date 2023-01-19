﻿using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CheckInProject.CheckInCore.Models;
using CheckInProject.CheckInCore.Interfaces;
using Microsoft.Win32;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// CheckInRecordsPage.xaml 的交互逻辑
    /// </summary>
    public partial class CheckInRecordsPage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public List<CheckInDataModels> ListBoxItems => CheckInManager.QueryTodayRecords();
        public CheckInRecordsPage(IServiceProvider provider)
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
                CheckInManager.ExportRecordsToExcelFile(ExportTypeEnum.CheckedIn, fileSaveDialog.FileName);
            }
        }
    }
}