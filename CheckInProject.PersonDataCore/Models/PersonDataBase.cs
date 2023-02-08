using Microsoft.EntityFrameworkCore;
using MiniExcelLibs.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CheckInProject.PersonDataCore.Models
{
    public class RawPersonDataBase
    {
        public string? Name { get; set; }
        public required double[] FaceEncoding { get; set; }
        public uint? ClassID { get; set; }
        public Guid StudentID { get; set; }
        public StringPersonDataBase ConvertToStringPersonDataBase()
        {
            var encodingResult = string.Join(";", FaceEncoding.Select(p => p.ToString()).ToArray());
            return new StringPersonDataBase { FaceEncodingString = encodingResult, Name = Name, ClassID = ClassID, StudentID = StudentID };
        }
    }
    public class StringPersonDataBase
    {
        [ExcelColumnName("学生姓名")]
        public string? Name { get; set; }
        [ExcelIgnore]
        public required string FaceEncodingString { get; set; }
        [ExcelColumnName("学号")]
        public uint? ClassID { get; set; }
        [ExcelIgnore]
        [Key]
        public Guid StudentID { get; set; }
        public RawPersonDataBase ConvertToRawPersonDataBase()
        {
            var encodingResult = Array.ConvertAll(FaceEncodingString.Split(';'), double.Parse);
            return new RawPersonDataBase { FaceEncoding = encodingResult, Name = Name, ClassID = ClassID, StudentID = StudentID };
        }
    }

    public class StringPersonDataBaseContext : DbContext
    {
        public DbSet<StringPersonDataBase> PersonData { get; set; }

        public string DbPath { get; }

        public StringPersonDataBaseContext()
        {
            var path = Environment.CurrentDirectory;
            var targetPath = Path.Join(path, "PersonData.db");
            DbPath = targetPath;
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath};Password=AccessDenied");
    }
}