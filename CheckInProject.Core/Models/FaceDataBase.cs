using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CheckInProject.Core.Models
{
    public class RawFaceDataBase
    {
        public string? Name { get; set; }
        public required double[] FaceEncoding { get; set; }
        public uint? PersonID { get; set; }
        public StringFaceDataBase ConvertToStringFaceDataBase()
        {
            var encodingResult = string.Join(";", FaceEncoding.Select(p => p.ToString()).ToArray());
            return new StringFaceDataBase { FaceEncodingString = encodingResult, Name = Name, PersonID = PersonID};            
        }
    }
    public class StringFaceDataBase
    {
        public string? Name { get; set; }
        [Key]
        public required string FaceEncodingString { get; set; }
        public uint? PersonID { get; set; }
        public RawFaceDataBase ConvertToRawFaceDataBase()
        {
            var encodingResult = Array.ConvertAll(FaceEncodingString.Split(';'), double.Parse);
            return new RawFaceDataBase { FaceEncoding= encodingResult, Name = Name, PersonID = PersonID};
        }
    }
    
    public class StringFaceDataBaseContext : DbContext
    {
        public DbSet<StringFaceDataBase> FaceData { get; set; }
        
        public string DbPath { get; }

        public StringFaceDataBaseContext()
        {
            var path = Environment.CurrentDirectory;
            var targetPath = Path.Join(path, "FaceData.db");
            DbPath = targetPath; 
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}