#region
using FaceRecognitionDotNet;
using System.Drawing;
using System.Text.Json;
#endregion
namespace CheckInProject.FaceDataCreator;
internal class Program
{
    static void Main()
    {
        var targetPath = Console.ReadLine();
        var FaceDataDictionary = new List<FaceDataBase>();
        if (!Directory.Exists("models"))
        {
            Console.WriteLine("Error: Cannot Find Model File!");
            return;
        }
        if (!string.IsNullOrWhiteSpace(targetPath) && Directory.Exists(targetPath))
        {
            var imageFiles = Directory.GetFiles(targetPath);
            using (var faceAPI = FaceRecognition.Create("models"))
            {
                foreach (var imageFile in imageFiles)
                {
                    FileInfo fileInfo = new FileInfo(imageFile);
                    if (fileInfo.Extension == ".jpg")
                    {
                        Console.WriteLine($"Creating Face Encoding For File {fileInfo.Name}.");
                        using (var imageBitmap = new Bitmap(imageFile))
                        {
                            using (var recognitionImage = FaceRecognition.LoadImage(imageBitmap))
                            {
                                var encoding = faceAPI.FaceEncodings(recognitionImage).First().GetRawEncoding();
                                var personName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
                                FaceDataDictionary.Add(new FaceDataBase { FaceEncoding = encoding, Name = personName });
                            }
                        }
                        Console.WriteLine($"Face Encoding For File {fileInfo.Name} Has Been Created.");
                    }
                }
            }
            Console.WriteLine("Start Saving Result File.");
            var result = JsonSerializer.Serialize(FaceDataDictionary);
            var streamWriter = new StreamWriter("FaceDataFile.json", false);
            streamWriter.Write(result);
            streamWriter.Dispose();
            Console.WriteLine("File Saved. \nPress Any Key To Exit.");
            Console.ReadLine();
        }
    }
}