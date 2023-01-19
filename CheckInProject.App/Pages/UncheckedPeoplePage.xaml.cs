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
    /// UncheckedPeoplePage.xaml 的交互逻辑
    /// </summary>
    public partial class UncheckedPeoplePage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public List<StringPersonDataBase> ListBoxItems => ServiceProvider.GetRequiredService<ICheckInManager>().ShowTodayUncheckedRecords();
        public UncheckedPeoplePage(IServiceProvider provider)
        {
            ServiceProvider = provider;
            InitializeComponent();
        }
    }
}