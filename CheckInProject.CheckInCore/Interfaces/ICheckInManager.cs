using CheckInProject.CheckInCore.Models;
using CheckInProject.PersonDataCore.Models;

namespace CheckInProject.CheckInCore.Interfaces
{
    public interface ICheckInManager
    {
        public Task CheckIn(DateOnly currentDate, TimeOnly currentTime, uint? personId);
        public List<CheckInDataModels> QueryTodayRecords();
        public List<StringPersonDataBase> QueryRequestedTimeUncheckedRecords(TimeEnum? targetTime);
        public Task ExportRecordsToExcelFile(ExportTypeEnum exportType, string path, TimeEnum? targetTime = null);
        public Task ClearCheckInRecords();
    }
}
