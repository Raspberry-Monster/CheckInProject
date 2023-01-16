#region
using CheckInProject.Abstraction.Models;
using FaceRecognitionDotNet;
using System.Drawing;
using System.Text.Json;
using CheckInProject.Core;
using Microsoft.Extensions.DependencyInjection;
using CheckInProject.Core.Interfaces;
using CheckInProject.Core.Implementation;
#endregion
namespace CheckInProject.FaceDataCreator;
internal class Program
{
    public static IServiceCollection ServiceCollections = new ServiceCollection();
    public static IServiceProvider ?ServiceProvider;
    static void Main()
    {
        if (!Directory.Exists("models"))
        {
            Console.WriteLine("Error: Cannot Find Model File!");
            return;
        }
        else
        {
            var faceRecognitionService = FaceRecognition.Create("models");
            var database = new StringFaceDataBaseContext();
            ServiceCollections.AddSingleton(faceRecognitionService);
            ServiceCollections.AddSingleton(database);
            ServiceProvider=ServiceCollections.BuildServiceProvider();
        }
        var targetPath = Console.ReadLine();
        var FaceDataList = new List<RawFaceDataBase>();
        
        if (!string.IsNullOrWhiteSpace(targetPath) && Directory.Exists(targetPath))
        {
            var imageFiles = Directory.GetFiles(targetPath);
            IFaceDataManager faceDataManager = new FaceDataManager(ServiceProvider);
            foreach (var imageFile in imageFiles)
            {
                FileInfo fileInfo = new FileInfo(imageFile);
                if (fileInfo.Extension == ".jpg")
                {
                    Console.WriteLine($"Creating Face Encoding For File {fileInfo.Name}.");
                    using (var imageBitmap = new Bitmap(imageFile))
                    {
                        var sourceName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                        var resultFaceData = faceDataManager.CreateFaceData(imageBitmap, sourceName);
                        FaceDataList.Add(resultFaceData);
                    }
                    Console.WriteLine($"Face Encoding For File {fileInfo.Name} Has Been Created.");
                }
            }
            var stringFaceDataList = FaceDataList.Select(t => t.ConvertToStringFaceDataBase()).ToList();
            IDatabaseManager databaseManager = new DatabaseManager(ServiceProvider);
            databaseManager.ImportFaceData(stringFaceDataList);
            Console.WriteLine("Start Saving Result File.");
            Console.WriteLine("File Saved. \nPress Any Key To Exit.");
            Console.ReadLine();
        }
    }
}