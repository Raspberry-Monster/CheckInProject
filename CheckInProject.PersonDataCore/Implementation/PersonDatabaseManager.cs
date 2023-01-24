using CheckInProject.PersonDataCore.Interfaces;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CheckInProject.PersonDataCore.Implementation
{
    public class PersonDatabaseManager : IPersonDatabaseManager
    {
        public StringPersonDataBaseContext DatabaseService => Provider.GetRequiredService<StringPersonDataBaseContext>();
        private readonly IServiceProvider Provider;
        public IList<StringPersonDataBase> GetFaceData()
        {
            var result = DatabaseService.PersonData.OrderBy(t => t.PersonID).ToList();
            return result;
        }

        public async Task ImportFaceData(IList<StringPersonDataBase> faceData)
        {
            await ClearFaceData();
            DatabaseService.AddRange(faceData);
            await DatabaseService.SaveChangesAsync();
        }

        public async Task ClearFaceData()
        {
            var currentFaceData = GetFaceData();
            if (currentFaceData.Count != 0)
            {
                DatabaseService.RemoveRange(currentFaceData);
                await DatabaseService.SaveChangesAsync();
            }
        }

        public PersonDatabaseManager(IServiceProvider provider)
        {
            Provider = provider;
        }
    }

}
