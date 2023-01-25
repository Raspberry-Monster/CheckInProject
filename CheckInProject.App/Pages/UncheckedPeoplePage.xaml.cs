using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Models;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// UncheckedPeoplePage.xaml 的交互逻辑
    /// </summary>
    public partial class UncheckedPeoplePage : Page, INotifyPropertyChanged
    {
        private readonly IServiceProvider ServiceProvider;
        public ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public List<StringPersonDataBase> UncheckedPeople
        {
            get => _uncheckedPeople;
            set
            {
                _uncheckedPeople = value;
                NotifyPropertyChanged();
            }
        }
        private List<StringPersonDataBase> _uncheckedPeople = new List<StringPersonDataBase>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public UncheckedPeoplePage(IServiceProvider provider)
        {
            ServiceProvider = provider;
            UncheckedPeople = CheckInManager.QueryRequestedTimeUncheckedRecords(null);
            InitializeComponent();
        }
        private async void ExportCheckInDataToExcelFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileSaveDialog = new SaveFileDialog();
                fileSaveDialog.Filter = "Excel文件|.xlsx";
                if (fileSaveDialog.ShowDialog() == true)
                {
                    TimeEnum currentTime;
                    if (QueryDuration.SelectedItem != null)
                    {
                        currentTime = (TimeEnum)QueryDuration.SelectedIndex;
                        await CheckInManager.ExportRecordsToExcelFile(ExportTypeEnum.UncheckedIn, fileSaveDialog.FileName, currentTime);
                    }
                    else await CheckInManager.ExportRecordsToExcelFile(ExportTypeEnum.UncheckedIn, fileSaveDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QueryDuration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TimeEnum currentTime;
                if (QueryDuration.SelectedItem != null)
                {
                    currentTime = (TimeEnum)QueryDuration.SelectedIndex;
                    UncheckedPeople = CheckInManager.QueryRequestedTimeUncheckedRecords(currentTime);
                }
                else UncheckedPeople = CheckInManager.QueryRequestedTimeUncheckedRecords(null);
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
    }
}