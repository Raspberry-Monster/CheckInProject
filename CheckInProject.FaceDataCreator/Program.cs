#region
using CheckInProject.Abstraction.Models;
using FaceRecognitionDotNet;
using System.Drawing;
using CheckInProject.Core;
using Microsoft.Extensions.DependencyInjection;
using CheckInProject.Core.Interfaces;
using CheckInProject.Core.Implementation;
using System.Security.Cryptography.X509Certificates;
#endregion
namespace CheckInProject.FaceDataCreator;
internal class Program
{
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
            ServiceProvider = ConfigureService();
        }
        var targetPath = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(targetPath) && Directory.Exists(targetPath))
        {
            var stringFaceDataList = CreateFaceData(targetPath).Select(t => t.ConvertToStringFaceDataBase()).ToList();
            ExportFaceDataToDatabase(stringFaceDataList);
            Console.WriteLine("Start Saving Result File.");
            Console.WriteLine("File Saved. \nPress Any Key To Exit.");
            Console.ReadLine();
        }
    }
    public static ServiceProvider ConfigureService()
    {
        var serviceCollections = new ServiceCollection();
        var faceRecognitionService = FaceRecognition.Create("models");
        serviceCollections.AddSingleton(faceRecognitionService);
        serviceCollections.AddSingleton<StringFaceDataBaseContext>();
        serviceCollections.AddTransient<IDatabaseManager, DatabaseManager>();
        serviceCollections.AddTransient<IFaceDataManager, FaceDataManager>();
        return serviceCollections.BuildServiceProvider();
    }
    public static IList<RawFaceDataBase> CreateFaceData(string targetPath)
    {
        var FaceDataList = new List<RawFaceDataBase>();
        var imageFiles = Directory.GetFiles(targetPath);
        foreach (var imageFile in imageFiles)
        {
            FileInfo fileInfo = new FileInfo(imageFile);
            if (fileInfo.Extension == ".jpg" && ServiceProvider != null)
            {
                var faceDataManager = ServiceProvider.GetRequiredService<IFaceDataManager>();
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
        return FaceDataList;
    }
    public static void ExportFaceDataToDatabase(IList<StringFaceDataBase> stringFaceDatas)
    {
        if ( ServiceProvider != null ) 
        {
            var databaseManager = ServiceProvider.GetRequiredService<IDatabaseManager>();
            databaseManager.ImportFaceData(stringFaceDatas);
        }
    }
}