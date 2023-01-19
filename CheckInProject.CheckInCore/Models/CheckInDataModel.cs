﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniExcelLibs.Attributes;

namespace CheckInProject.CheckInCore.Models
{
    public class CheckInDataModels
    {
        [ExcelColumnName("学生姓名")]
        public string? Name { get; set; }
        [ExcelColumnName("学号")]
        public uint? PersonID { get; set; }
        [ExcelIgnore]
        [Key]
        public Guid RecordID { get; set; }
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
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
