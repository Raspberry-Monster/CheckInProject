using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Models;
using CheckInProject.CheckInCore.Utils;
using CheckInProject.PersonDataCore.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInProject.CheckInCore.Implementation
{
    public class CheckInManager : ICheckInManager
    {
        public CheckInDataModelContext CheckInDatabaseService => Provider.GetRequiredService<CheckInDataModelContext>();
        public StringPersonDataBaseContext PersonDatabaseService => Provider.GetRequiredService<StringPersonDataBaseContext>();
        public IServiceProvider Provider;
        public CheckInManager(IServiceProvider provider)
        {
            Provider = provider;
        }

        public async Task<bool> CheckIn(DateOnly currentDate, TimeOnly currentTime, uint? personId)
        {
            var checkInHistory = CheckInDatabaseService.CheckInData.ToList().Find(t => t.CheckInDate == currentDate && t.PersonID == personId);
            if (checkInHistory != null)
            {
                var currentTimeDescription = TimeConverter.ConvertTimeToDescription(currentTime);
                switch(currentTimeDescription)
                {
                    case TimeEnum.Morning:
                        checkInHistory.MorningCheckedIn = true;
                        checkInHistory.MorningCheckInTime = currentTime;
                        break;
                    case TimeEnum.Afternoon:
                        checkInHistory.AfternoonCheckedIn = true;
                        checkInHistory.AfternoonCheckInTime= currentTime;
                        break;
                    case TimeEnum.Evening:
                        checkInHistory.EveningCheckedIn = true;
                        checkInHistory.EveningCheckInTime = currentTime;
                        break;
                }
                CheckInDatabaseService.Update(checkInHistory);
                await CheckInDatabaseService.SaveChangesAsync();
                return true;
            }
            else
            {
                var personData = PersonDatabaseService.PersonData.ToList().Find(t => t.PersonID == personId);
                var currentTimeDescription = TimeConverter.ConvertTimeToDescription(currentTime);
                var checkInData = new CheckInDataModels
                {
                    Name = personData?.Name,
                    CheckInDate = currentDate,
                    PersonID = personId
                };
                switch (currentTimeDescription)
                {
                    case TimeEnum.Morning:
                        checkInData.MorningCheckedIn = true;
                        checkInData.MorningCheckInTime = currentTime;
                        break;
                    case TimeEnum.Afternoon:
                        checkInData.AfternoonCheckedIn = true;
                        checkInData.AfternoonCheckInTime = currentTime;
                        break;
                    case TimeEnum.Evening:
                        checkInData.EveningCheckedIn = true;
                        checkInData.EveningCheckInTime = currentTime;
                        break;
                }
                CheckInDatabaseService.Add(checkInData);
                await CheckInDatabaseService.SaveChangesAsync();
                return true;
            }
        }

        public List<CheckInDataModels> ShowTodayRecords()
        {
            var result = CheckInDatabaseService.CheckInData.AsEnumerable().Where(t => t.CheckInDate == DateOnly.FromDateTime(DateTime.Now)).ToList();
            return result;
        }
        public List<StringPersonDataBase> ShowTodayUncheckedRecords()
        {
            var checkedInRecordsPersonID = CheckInDatabaseService.CheckInData.AsEnumerable()
                    .Where(t => t.CheckInDate == DateOnly.FromDateTime(DateTime.Now)).Select(t => t.PersonID).ToList();
            var checkedPeople = PersonDatabaseService.PersonData.AsEnumerable().Where(t => checkedInRecordsPersonID.Contains(t.PersonID)).ToList();
            var uncheckedPeople = PersonDatabaseService.PersonData.AsEnumerable().ToList();
            checkedPeople.ForEach(t => uncheckedPeople.Remove(t));
            return uncheckedPeople;
        }

        public void ExportRecordsToExcelFile()
        {
            throw new NotImplementedException();
        }
    }
}
