using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInProject.CheckInCore.Models
{
    public class CheckInDataModels
    {
        public string? Name { get; set; }
        public uint? PersonID { get; set; }
        [Key]
        public  Guid RecordID { get; set; }
        public bool MorningCheckedIn { get; set; } = false;
        public TimeOnly? MorningCheckInTime { get; set; }
        public bool AfternoonCheckedIn { get; set; } = false;
        public TimeOnly? AfternoonCheckInTime { get; set; }
        public bool EveningCheckedIn { get; set; } = false;
        public TimeOnly? EveningCheckInTime { get; set; }
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
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
