using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckInProject.PersonDataCore.Models;
using CheckInProject.PersonDataCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CheckInProject.PersonDataCore.Implementation
{
    public class PersonDatabaseManager : IPersonDatabaseManager
    {
        public StringPersonDataBaseContext DatabaseService => Provider.GetRequiredService<StringPersonDataBaseContext>();
        public IServiceProvider Provider;
        public IList<StringPersonDataBase> GetFaceData()
        {
            var result = DatabaseService.FaceData.ToList();
            return result;
        }

        public void ImportFaceData(IList<StringPersonDataBase> faceData)
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

        public PersonDatabaseManager(IServiceProvider provider)
        {
            Provider = provider;
        }
    }

}
