using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CheckInProject.CheckInCore.Models;
using CheckInProject.CheckInCore.Interfaces;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// CheckInRecordsPage.xaml 的交互逻辑
    /// </summary>
    public partial class CheckInRecordsPage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public List<CheckInDataModels> ListBoxItems => ServiceProvider.GetRequiredService<ICheckInManager>().ShowTodayRecords();
        public CheckInRecordsPage(IServiceProvider provider)
        {
            ServiceProvider = provider;
            InitializeComponent();
        }
    }
}