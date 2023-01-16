using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckInProject.Abstraction.Models;
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
