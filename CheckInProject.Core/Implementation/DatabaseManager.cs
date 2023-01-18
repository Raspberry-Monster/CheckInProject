using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckInProject.Core.Models;
using CheckInProject.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
            ClearFaceData();
            DatabaseService.AddRange(faceData);
            DatabaseService.SaveChanges();
        }

        public void ClearFaceData()
        {
            var currentFaceData = GetFaceData();
            if (currentFaceData.Count != 0)
            {
                DatabaseService.RemoveRange(currentFaceData);
            }
        }

        public DatabaseManager(IServiceProvider provider)
        {
            Provider = provider;
        }
    }

}
