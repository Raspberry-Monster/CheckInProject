using System.Drawing;
using CheckInProject.Core.Models;
using FaceRecognitionDotNet;
namespace CheckInProject.Core.Interfaces
{
    public interface IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI{ get; }
        public RawFaceDataBase CreateFaceData(Bitmap sourceData, string? sourceName, uint? personID);
        public IList<RawFaceDataBase> CreateFacesData(Bitmap sourceData);
        public IList<RawFaceDataBase> CompareFace(IList<RawFaceDataBase> faceDataList, RawFaceDataBase targetFaceData);
        public IList<RawFaceDataBase> CompareFaces(IList<RawFaceDataBase> faceDataList, IList<RawFaceDataBase> targetFaceDataList);
    }
}
