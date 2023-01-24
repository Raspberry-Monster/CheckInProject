using CheckInProject.App.Pages;
using CheckInProject.CheckInCore.Implementation;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Models;
using CheckInProject.PersonDataCore.Implementation;
using CheckInProject.PersonDataCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using FaceRecognitionDotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CheckInProject.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly IServiceProvider ServiceProvider;

        public static Frame? RootFrame = null;
        public App()
        {
            var service = new ServiceCollection();
            //For Databases
            service.AddDbContext<StringPersonDataBaseContext>();
            service.AddDbContext<CheckInDataModelContext>();
            //For services
            var faceRecognitionService = FaceRecognition.Create("FaceRecognitionModel");
            service.AddSingleton(faceRecognitionService);
            service.AddSingleton<IPersonDatabaseManager, PersonDatabaseManager>();
            service.AddSingleton<ICheckInManager, CheckInManager>();
            service.AddSingleton<IFaceDataManager, FaceDataManager>();
            service.AddSingleton<List<RawPersonDataBase>>();
            // For UI
            service.AddSingleton<MainWindow>();
            service.AddSingleton<ScanStaticPicturePage>();
            service.AddTransient<CheckInRecordsPage>();
            service.AddSingleton<ScanDynamicPicturePage>();
            service.AddTransient<FaceDataManagementPage>();
            service.AddTransient<MultipleResultsPage>();
            service.AddTransient<UncheckedPeoplePage>();
            service.AddTransient<DatabaseManagementPage>();
            ServiceProvider = service.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ServiceProvider.GetRequiredService<MainWindow>()?.Show();
        }
    }
}
