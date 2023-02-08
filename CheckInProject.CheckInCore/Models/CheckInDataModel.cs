using Microsoft.EntityFrameworkCore;
using MiniExcelLibs.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CheckInProject.CheckInCore.Models
{
    public class CheckInDataModels
    {
        [Key]
        public Guid RecordID { get; set; }
        public Guid StudentID { get; set; }
        public bool MorningCheckedIn { get; set; } = false;
        public TimeOnly? MorningCheckInTime { get; set; }
        public bool AfternoonCheckedIn { get; set; } = false;
        public TimeOnly? AfternoonCheckInTime { get; set; }
        public bool EveningCheckedIn { get; set; } = false;
        public TimeOnly? EveningCheckInTime { get; set; }
        public DateOnly CheckInDate { get; set; }
        public CheckInDataExportModels GetCheckInDataExportModels()
        {
            var result = new CheckInDataExportModels()
            {
                MorningCheckedIn = MorningCheckedIn,
                AfternoonCheckedIn = AfternoonCheckedIn,
                EveningCheckedIn = EveningCheckedIn,
                MorningCheckInTime = MorningCheckInTime,
                AfternoonCheckInTime = AfternoonCheckInTime,
                EveningCheckInTime = EveningCheckInTime,
                CheckInDate = CheckInDate,
                StudentID = StudentID
            };
            return result;
        }
    }
    public class CheckInDataExportModels
    {
        [ExcelColumnName("学生姓名")]
        public string? Name { get; set; }
        [ExcelColumnName("学号")]
        public uint? ClassID { get; set; }
        [ExcelIgnore]
        public Guid StudentID { get; set; }
        [ExcelColumnName("早上打卡情况")]
        public bool MorningCheckedIn { get; set; } = false;
        [ExcelColumnName("早上打卡时间")]
        public TimeOnly? MorningCheckInTime { get; set; }
        [ExcelColumnName("下午打卡情况")]
        public bool AfternoonCheckedIn { get; set; } = false;
        [ExcelColumnName("下午打卡时间")]
        public TimeOnly? AfternoonCheckInTime { get; set; }
        [ExcelColumnName("晚上打卡情况")]
        public bool EveningCheckedIn { get; set; } = false;
        [ExcelColumnName("晚上打卡时间")]
        public TimeOnly? EveningCheckInTime { get; set; }
        [ExcelColumnName("打卡日期")]
        public DateOnly CheckInDate { get; set; }
    }
    public class CheckInDataModelContext : DbContext
    {
        public DbSet<CheckInDataModels> CheckInData { get; set; }

        public string DbPath { get; }

        public CheckInDataModelContext()
        {
            var path = Environment.CurrentDirectory;
            var targetPath = Path.Join(path, "CheckInData.db");
            DbPath = targetPath;
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath};Password=AccessDenied");
    }
}
