using CheckInProject.Abstraction.Models;
using CheckInProject.Core.Interfaces;
using FaceRecognitionDotNet;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace CheckInProject.Core.Implementation
{
    public class FaceDataManager : IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI => Provider.GetRequiredService<FaceRecognition>();

        public IServiceProvider Provider;

        public RawFaceDataBase CreateFaceData(Bitmap sourceData,string sourceName )
        {
            using (var recognitionImage = FaceRecognition.LoadImage(sourceData))
            {
                var encoding = FaceRecognitionAPI.FaceEncodings(recognitionImage).First().GetRawEncoding();
                var personName = sourceName;
                return new RawFaceDataBase { FaceEncoding = encoding, Name = personName };
            }
        }

        public IList<string> CompareFaces(IList<RawFaceDataBase> faceDataList, RawFaceDataBase targetFaceData)
        {
            var faceEncodingList = faceDataList.Select(t => FaceRecognition.LoadFaceEncoding(t.FaceEncoding)).ToList();
            var targetFaceEncoding = FaceRecognition.LoadFaceEncoding(targetFaceData.FaceEncoding);
            var recognizedFaces = FaceRecognition.CompareFaces(faceEncodingList, targetFaceEncoding);
            var reconizedNames= new List<string>();
            var index = 0;
            foreach (var recognizedFace in recognizedFaces)
            {
                if (recognizedFace)
                {
                    var resultName = faceDataList[index].Name;
                    if (!string.IsNullOrEmpty(resultName))reconizedNames.Add(resultName);
                }
                index++;
            }
            return reconizedNames;
        }

        public IList<RawFaceDataBase> CreateFacesData(Bitmap sourceData)
        {
            using (var recognitionImage = FaceRecognition.LoadImage(sourceData))
            {
                var encodings = FaceRecognitionAPI.FaceEncodings(recognitionImage).Select(t => new RawFaceDataBase { FaceEncoding = t.GetRawEncoding() }).ToList();
                return encodings;
            }
        }

        public FaceDataManager(IServiceProvider serviceProvider)
        {
            Provider = serviceProvider;
        }
    }
}
