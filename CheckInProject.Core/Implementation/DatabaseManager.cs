using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckInProject.Abstraction.Models;
using CheckInProject.Core.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace CheckInProject.Core.Implementation
{
    public class DatabaseManager : IDatabaseManager
    {
        public StringFaceDataBaseContext DatabaseService => Provider.GetRequiredService<StringFaceDataBaseContext>();
        public IServiceProvider Provider;
        public IList<StringFaceDataBase> GetFaceData()
        {
            var result = DatabaseService.FaceData.ToList();
            return result;
        }

        public void ImportFaceData(IList<StringFaceDataBase> faceData)
        {
            DatabaseService.AddRange(faceData);
            DatabaseService.SaveChanges();
        }
        public DatabaseManager(IServiceProvider provider)
        {
            Provider = provider;
        }
    }

}
