﻿using CheckInProject.Core.Models;
using CheckInProject.App.Pages;
using CheckInProject.Core.Implementation;
using CheckInProject.Core.Interfaces;
using FaceRecognitionDotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;

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
            //For services
            var faceRecognitionService = FaceRecognition.Create("models");
            service.AddSingleton(faceRecognitionService);
            service.AddSingleton<StringFaceDataBaseContext>();
            service.AddSingleton<IDatabaseManager, DatabaseManager>();
            service.AddSingleton<IFaceDataManager, FaceDataManager>();
            service.AddSingleton<List<RawFaceDataBase>>();
            // For UI
            service.AddSingleton<MainWindow>();
            service.AddTransient<ScanStaticPicturePage>();
            service.AddTransient<HistoryPage>();
            service.AddTransient<ScanDynamicPicturePage>();
            service.AddTransient<FaceDataManagementPage>();
            service.AddTransient<MultipleResultsPage>();
            ServiceProvider = service.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ServiceProvider.GetRequiredService<MainWindow>()?.Show();
        }
    }
}
