using CheckInProject.CheckInCore.Interfaces;
using CheckInProject.CheckInCore.Models;
using CheckInProject.CheckInCore.Utils;
using CheckInProject.PersonDataCore.Models;
using Microsoft.Extensions.DependencyInjection;
using MiniExcelLibs;

namespace CheckInProject.CheckInCore.Implementation
{
    public class CheckInManager : ICheckInManager
    {
        public CheckInDataModelContext CheckInDatabaseService => Provider.GetRequiredService<CheckInDataModelContext>();
        public StringPersonDataBaseContext PersonDatabaseService => Provider.GetRequiredService<StringPersonDataBaseContext>();
        private readonly IServiceProvider Provider;
        public CheckInManager(IServiceProvider provider)
        {
            Provider = provider;
        }

        public async Task CheckIn(DateOnly currentDate, TimeOnly currentTime, Guid studentId)
        {
            var checkInHistory = CheckInDatabaseService.CheckInData.ToList().Find(t => t.CheckInDate == currentDate && t.StudentID == studentId);
            if (checkInHistory != null)
            {
                var currentTimeDescription = TimeConverter.ConvertTimeToDescription(currentTime);
                switch (currentTimeDescription)
                {
                    case TimeEnum.Morning:
                        checkInHistory.MorningCheckedIn = true;
                        checkInHistory.MorningCheckInTime = currentTime;
                        break;
                    case TimeEnum.Afternoon:
                        checkInHistory.AfternoonCheckedIn = true;
                        checkInHistory.AfternoonCheckInTime = currentTime;
                        break;
                    case TimeEnum.Evening:
                        checkInHistory.EveningCheckedIn = true;
                        checkInHistory.EveningCheckInTime = currentTime;
                        break;
                }
                CheckInDatabaseService.Update(checkInHistory);
                await CheckInDatabaseService.SaveChangesAsync();
            }
            else
            {
                var personData = PersonDatabaseService.PersonData.ToList().Find(t => t.StudentID == studentId);
                var currentTimeDescription = TimeConverter.ConvertTimeToDescription(currentTime);
                var checkInData = new CheckInDataModels
                {
                    CheckInDate = currentDate,
                    StudentID = studentId
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
            }
        }

        public List<CheckInDataExportModels> QueryTodayRecords()
        {
            var sourceData = CheckInDatabaseService.CheckInData.AsEnumerable().Where(t => t.CheckInDate == DateOnly.FromDateTime(DateTime.Now)).ToList();
            var resultList = sourceData.Select(t => t.GetCheckInDataExportModels()).ToList();
            foreach (var item in resultList)
            {
                var personData = PersonDatabaseService.PersonData.AsEnumerable().Where(t => t.StudentID == item.StudentID).First();
                item.Name = personData.Name;
                item.ClassID = personData.ClassID;
            }
            return resultList;
        }
        public List<StringPersonDataBase> QueryRequestedTimeUncheckedRecords(TimeEnum? targetTime)
        {
            var todaycheckInRecords = CheckInDatabaseService.CheckInData.AsEnumerable().Where(t => t.CheckInDate == DateOnly.FromDateTime(DateTime.Now)).ToList();
            IList<Guid> currentTimeCheckedInRecords;
            if (targetTime == null) targetTime = TimeConverter.ConvertTimeToDescription(TimeOnly.FromDateTime(DateTime.Now));
            switch (targetTime)
            {
                case TimeEnum.Morning:
                    currentTimeCheckedInRecords = todaycheckInRecords.Where(t => t.MorningCheckedIn == true).Select(t => t.StudentID).ToList();
                    break;
                case TimeEnum.Afternoon:
                    currentTimeCheckedInRecords = todaycheckInRecords.Where(t => t.AfternoonCheckedIn == true).Select(t => t.StudentID).ToList();
                    break;
                case TimeEnum.Evening:
                    currentTimeCheckedInRecords = todaycheckInRecords.Where(t => t.EveningCheckedIn == true).Select(t => t.StudentID).ToList();
                    break;
                default:
                    currentTimeCheckedInRecords = new List<Guid>();
                    break;
            }
            var checkedPeople = PersonDatabaseService.PersonData.AsEnumerable().Where(t => currentTimeCheckedInRecords.Contains(t.StudentID)).ToList();
            var uncheckedPeople = PersonDatabaseService.PersonData.AsEnumerable().OrderBy(t => t.ClassID).ToList();
            checkedPeople.ForEach(t => uncheckedPeople.Remove(t));
            return uncheckedPeople;
        }

        public async Task ExportRecordsToExcelFile(ExportTypeEnum exportType, string path, TimeEnum? targetTime = null)
        {
            if (exportType == ExportTypeEnum.UncheckedIn)
            {
                var uncheckedInData = QueryRequestedTimeUncheckedRecords(targetTime);
                await MiniExcel.SaveAsAsync(path, uncheckedInData, overwriteFile: true);
            }
            else
            {
                var checkedInData = QueryTodayRecords();
                await MiniExcel.SaveAsAsync(path, checkedInData, overwriteFile: true);
            }
        }

        public async Task ClearCheckInRecords()
        {
            var currentCheckInRecords = CheckInDatabaseService.CheckInData.ToList();
            CheckInDatabaseService.RemoveRange(currentCheckInRecords);
            await CheckInDatabaseService.SaveChangesAsync();
        }
    }
}
