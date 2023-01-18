using CheckInProject.Core.Models;
using CheckInProject.Core.Interfaces;
using FaceRecognitionDotNet;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;

namespace CheckInProject.Core.Implementation
{
    public class FaceDataManager : IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI => Provider.GetRequiredService<FaceRecognition>();

        public IServiceProvider Provider;

        public RawFaceDataBase CreateFaceData(Bitmap sourceData, string? sourceName )
        {
            using (var recognitionImage = FaceRecognition.LoadImage(sourceData))
            {
                var encoding = FaceRecognitionAPI.FaceEncodings(recognitionImage).First().GetRawEncoding();
                var personName = sourceName;
                return new RawFaceDataBase { FaceEncoding = encoding, Name = personName };
            }
        }
        public IList<RawFaceDataBase> CreateFacesData(Bitmap sourceData)
        {
            using (var recognitionImage = FaceRecognition.LoadImage(sourceData))
            {
                var encodings = FaceRecognitionAPI.FaceEncodings(recognitionImage).Select(t => new RawFaceDataBase { FaceEncoding = t.GetRawEncoding() }).ToList();
                return encodings;
            }
        }

        public IList<RawFaceDataBase> CompareFace(IList<RawFaceDataBase> faceDataList, RawFaceDataBase targetFaceData)
        {
            var faceEncodingList = faceDataList.Select(t => FaceRecognition.LoadFaceEncoding(t.FaceEncoding)).ToList();
            var targetFaceEncoding = FaceRecognition.LoadFaceEncoding(targetFaceData.FaceEncoding);
            var recognizedFaces = FaceRecognition.CompareFaces(faceEncodingList, targetFaceEncoding, 0.5);
            var reconizedNames= new List<RawFaceDataBase>();
            var index = 0;
            foreach (var recognizedFace in recognizedFaces)
            {
                if (recognizedFace)
                {
                    var resultName = faceDataList[index];
                    reconizedNames.Add(resultName);
                }
                index++;
            }
            return reconizedNames;
        }
        public IList<RawFaceDataBase> CompareFaces(IList<RawFaceDataBase> faceDataList, IList<RawFaceDataBase> targetFaceDataList)
        {
            var faceEncodingList = faceDataList.Select(t => FaceRecognition.LoadFaceEncoding(t.FaceEncoding)).ToList();
            var reconizedNames = new List<RawFaceDataBase>();
            foreach (var targetFaceData in targetFaceDataList)
            {
                var targetFaceEncoding = FaceRecognition.LoadFaceEncoding(targetFaceData.FaceEncoding);
                var recognizedFaces = FaceRecognition.CompareFaces(faceEncodingList, targetFaceEncoding, 0.5);
                var index = 0;
                foreach (var recognizedFace in recognizedFaces)
                {
                    if (recognizedFace)
                    {
                        var resultName = faceDataList[index];
                        reconizedNames.Add(resultName);
                    }
                    index++;
                }
            }
            return reconizedNames;
        }

        public FaceDataManager(IServiceProvider serviceProvider)
        {
            Provider = serviceProvider;
        }
    }
}
