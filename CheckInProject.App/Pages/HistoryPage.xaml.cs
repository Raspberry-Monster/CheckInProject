using System;
using System.Windows.Controls;

namespace CheckInProject.App.Pages
{
    /// <summary>
    /// HistoryPage.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryPage : Page
    {
        private readonly IServiceProvider ServiceProvider;
        public HistoryPage(IServiceProvider provider)
        {
            ServiceProvider = provider;
            InitializeComponent();
        }
    }
}
