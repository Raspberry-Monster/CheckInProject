using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// MultipleResultsPage.xaml 的交互逻辑
    /// </summary>
    public partial class MultipleResultsPage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public List<RawPersonDataBase> ListBoxItems => ServiceProvider.GetRequiredService<List<RawPersonDataBase>>();

        public MultipleResultsPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void NamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var targetData = (sender as ListBox)?.SelectedItem as RawPersonDataBase;
                if (targetData!= null)
                {
                    var currentTime = DateTime.Now;
                    await CheckInManager.CheckIn(DateOnly.FromDateTime(currentTime), TimeOnly.FromDateTime(currentTime), targetData.StudentID);
                    App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
