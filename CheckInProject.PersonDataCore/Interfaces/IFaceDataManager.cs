using CheckInProject.PersonDataCore.Models;
using FaceRecognitionDotNet;
using System.Drawing;
namespace CheckInProject.PersonDataCore.Interfaces
{
    public interface IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI { get; }
        public RawPersonDataBase CreateFaceData(Bitmap sourceData, string? sourceName, uint? personID);
        public IList<RawPersonDataBase> CreateFacesData(Bitmap sourceData);
        public IList<RawPersonDataBase> CompareFace(IList<RawPersonDataBase> faceDataList, RawPersonDataBase targetFaceData);
        public IList<RawPersonDataBase> CompareFaces(IList<RawPersonDataBase> faceDataList, IList<RawPersonDataBase> targetFaceDataList);
    }
}
