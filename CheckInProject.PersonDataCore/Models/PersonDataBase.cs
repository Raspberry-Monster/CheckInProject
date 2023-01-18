using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckInProject.PersonDataCore.Models
{
    public class RawPersonDataBase
    {
        public string? Name { get; set; }
        public required double[] FaceEncoding { get; set; }
        public uint? PersonID { get; set; }
        public required Guid RecordID { get; set; }
        public StringPersonDataBase ConvertToStringPersonDataBase()
        {
            var encodingResult = string.Join(";", FaceEncoding.Select(p => p.ToString()).ToArray());
            return new StringPersonDataBase { FaceEncodingString = encodingResult, Name = Name, PersonID = PersonID, RecordID = RecordID };          
        }
    }
    public class StringPersonDataBase
    {
        public string? Name { get; set; }
        public required string FaceEncodingString { get; set; }
        public uint? PersonID { get; set; }
        [Key]
        public required Guid RecordID { get; set; }
        public RawPersonDataBase ConvertToRawPersonDataBase()
        {
            var encodingResult = Array.ConvertAll(FaceEncodingString.Split(';'), double.Parse);
            return new RawPersonDataBase { FaceEncoding = encodingResult, Name = Name, PersonID = PersonID, RecordID = RecordID };
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
            => options.UseSqlite($"Data Source={DbPath}");
    }
}