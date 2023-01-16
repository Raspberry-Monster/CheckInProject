using System.Drawing;
using CheckInProject.Core.Models;
using FaceRecognitionDotNet;
namespace CheckInProject.Core.Interfaces
{
    public interface IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI{ get; }
        public RawFaceDataBase CreateFaceData(Bitmap sourceData, string sourceName);
        public IList<RawFaceDataBase> CreateFacesData(Bitmap sourceData);
        public IList<string> CompareFaces(IList<RawFaceDataBase> faceDataList, RawFaceDataBase targetFaceData);
    }
}
