using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInProject.CheckInCore.Interfaces
{
    public interface ICheckInManager
    {
        public bool CheckIn();
        public void ExportToExcelFile();
    }
}
