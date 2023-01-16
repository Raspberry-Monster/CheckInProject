using CheckInProject.Core.Models;

namespace CheckInProject.Core.Interfaces
{
    public interface IDatabaseManager
    {
        public StringFaceDataBaseContext DatabaseService { get; }
        public IList<StringFaceDataBase> GetFaceData();
        public void ImportFaceData(IList<StringFaceDataBase> faceData);
        
    }
}
