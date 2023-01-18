using CheckInProject.PersonDataCore.Models;

namespace CheckInProject.PersonDataCore.Interfaces
{
    public interface IPersonDatabaseManager
    {
        public StringPersonDataBaseContext DatabaseService { get; }
        public IList<StringPersonDataBase> GetFaceData();
        public void ImportFaceData(IList<StringPersonDataBase> faceData);
        public void ClearFaceData();
        
    }
}
