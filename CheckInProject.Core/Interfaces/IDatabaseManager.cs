using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckInProject.Abstraction.Models;
using Microsoft.Data.Sqlite;

namespace CheckInProject.Core.Interfaces
{
    public interface IDatabaseManager
    {
        public StringFaceDataBaseContext DatabaseService { get; }
        public IList<StringFaceDataBase> GetFaceData();
        public void ImportFaceData(IList<StringFaceDataBase> faceData);
        
    }
}
