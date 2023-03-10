using CheckInProject.App.Models;
using CheckInProject.App.Pages;
using CheckInProject.CheckInCore.Implementation;
using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Models;
using CheckInProject.PersonDataCore.Implementation;
using CheckInProject.PersonDataCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using FaceRecognitionDotNet;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CheckInProject.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly IServiceProvider ServiceProvider;

        public static Frame? RootFrame = null;

        public static bool DisableDatabaseProtection = false;
        public App()
        {
            var service = new ServiceCollection();
            //For Databases
            service.AddDbContext<StringPersonDataBaseContext>();
            service.AddDbContext<CheckInDataModelContext>();
            //For services
            var faceRecognitionService = FaceRecognition.Create("FaceRecognitionModel");
            service.AddSingleton(faceRecognitionService);
            CascadeClassifier cascadeService = new CascadeClassifier("haarcascade_frontalface_alt.xml");
            service.AddSingleton(cascadeService);
            var applicationSettings = Settings.CreateSettings();
            service.AddSingleton(applicationSettings);
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
            service.AddTransient<SetDatabasePasswordPage>();
            ServiceProvider = service.BuildServiceProvider();
            App.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            foreach (var item in e.Args)
            {
                if (item == "--DisableDatabaseProtection") DisableDatabaseProtection = true;
            }
            ServiceProvider.GetRequiredService<StringPersonDataBaseContext>().Database.EnsureCreated();
            ServiceProvider.GetRequiredService<CheckInDataModelContext>().Database.EnsureCreated();
            ServiceProvider.GetRequiredService<MainWindow>()?.Show();
            var settings = ServiceProvider.GetRequiredService<Settings>();
            if (settings.IsFirstRun && string.IsNullOrEmpty(settings.PasswordMD5)) RootFrame?.Navigate(ServiceProvider.GetRequiredService<SetDatabasePasswordPage>());
        }
        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"发生了未经处理的异常:\n{e.Exception.Message}", "未经处理的异常", MessageBoxButton.OK,
                             MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
