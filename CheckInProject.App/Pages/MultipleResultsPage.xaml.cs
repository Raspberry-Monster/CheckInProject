using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// MultipleResultsPage.xaml 的交互逻辑
    /// </summary>
    public partial class MultipleResultsPage : Page
    {
        public IServiceProvider ServiceProvider;
        public ICheckInManager CheckInManager => ServiceProvider.GetRequiredService<ICheckInManager>();
        public List<RawPersonDataBase> ListBoxItems=>ServiceProvider.GetRequiredService<List<RawPersonDataBase>>();

        public MultipleResultsPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void NamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var targetData = (sender as ListBox)?.SelectedItem;
            if (targetData != null)
            {
                var currentTime = DateTime.Now;
                await CheckInManager.CheckIn(DateOnly.FromDateTime(currentTime), TimeOnly.FromDateTime(currentTime), (targetData as RawPersonDataBase)?.PersonID);
                App.RootFrame?.Navigate(ServiceProvider.GetRequiredService<CheckInRecordsPage>());
            }
        }
    }
}
