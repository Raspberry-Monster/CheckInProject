using CheckInProject.PersonDataCore.Models;
using CheckInProject.PersonDataCore.Interfaces;
using FaceRecognitionDotNet;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;

namespace CheckInProject.PersonDataCore.Implementation
{
    public class FaceDataManager : IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI => Provider.GetRequiredService<FaceRecognition>();

        public IServiceProvider Provider;

        public RawPersonDataBase CreateFaceData(Bitmap sourceData, string? sourceName, uint? personID )
        {
            using (var recognitionImage = FaceRecognition.LoadImage(sourceData))
            {
                var encoding = FaceRecognitionAPI.FaceEncodings(recognitionImage).First().GetRawEncoding();
                var personName = sourceName;
                return new RawPersonDataBase { FaceEncoding = encoding, Name = personName ,PersonID = personID};
            }
        }
        public IList<RawPersonDataBase> CreateFacesData(Bitmap sourceData)
        {
            using (var recognitionImage = FaceRecognition.LoadImage(sourceData))
            {
                var encodings = FaceRecognitionAPI.FaceEncodings(recognitionImage).Select(t => new RawPersonDataBase { FaceEncoding = t.GetRawEncoding() }).ToList();
                return encodings;
            }
        }

        public IList<RawPersonDataBase> CompareFace(IList<RawPersonDataBase> faceDataList, RawPersonDataBase targetFaceData)
        {
            var faceEncodingList = faceDataList.Select(t => FaceRecognition.LoadFaceEncoding(t.FaceEncoding)).ToList();
            var targetFaceEncoding = FaceRecognition.LoadFaceEncoding(targetFaceData.FaceEncoding);
            var recognizedFaces = FaceRecognition.CompareFaces(faceEncodingList, targetFaceEncoding, 0.5);
            var reconizedNames= new List<RawPersonDataBase>();
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
        public IList<RawPersonDataBase> CompareFaces(IList<RawPersonDataBase> faceDataList, IList<RawPersonDataBase> targetFaceDataList)
        {
            var faceEncodingList = faceDataList.Select(t => FaceRecognition.LoadFaceEncoding(t.FaceEncoding)).ToList();
            var reconizedNames = new List<RawPersonDataBase>();
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
