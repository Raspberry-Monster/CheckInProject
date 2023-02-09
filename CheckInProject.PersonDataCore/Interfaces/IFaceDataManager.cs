using CheckInProject.PersonDataCore.Models;
using FaceRecognitionDotNet;
using OpenCvSharp;
using System.Drawing;
namespace CheckInProject.PersonDataCore.Interfaces
{
    public interface IFaceDataManager
    {
        public FaceRecognition FaceRecognitionAPI { get; }
        public RawPersonDataBase CreateFaceData(Bitmap sourceData, string? sourceName, uint? personID);
        public IList<RawPersonDataBase> CreateFacesData(IList<Bitmap> sourceData);
        public IList<RawPersonDataBase> CompareFace(IList<RawPersonDataBase> faceDataList, RawPersonDataBase targetFaceData);
        public IList<RawPersonDataBase> CompareFaces(IList<RawPersonDataBase> faceDataList, IList<RawPersonDataBase> targetFaceDataList);
        public FaceCountModels GetFaceCount(Mat sourceImage);
        public FaceRetangleModels GetFaceImage(Bitmap sourceBitmap);
        public FaceRetangleModels GetFacesImage(Bitmap sourceBitmap);
    }
}
