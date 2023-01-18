using CheckInProject.CheckInCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInProject.CheckInCore.Interfaces
{
    public interface ICheckInManager
    {
        public Task<bool> CheckIn(DateOnly currentDate, TimeOnly currentTime, uint? personId);
        public List<CheckInDataModels> ShowData();
        public void ExportDataToExcelFile();
    }
}
