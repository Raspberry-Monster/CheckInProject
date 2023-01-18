using CheckInProject.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
