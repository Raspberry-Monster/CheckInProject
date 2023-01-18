using CheckInProject.Core.Models;
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
        public List<RawFaceDataBase> ListBoxItems=>ServiceProvider.GetRequiredService<List<RawFaceDataBase>>();

        public MultipleResultsPage(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            InitializeComponent();
        }

        private void NamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
