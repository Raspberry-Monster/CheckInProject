
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace CheckInProject.Abstraction.Models
{
    public class RawFaceDataBase
    {
        public string? Name { get; set; }
        public required double[] FaceEncoding { get; set; }
        public StringFaceDataBase ConvertToStringFaceDataBase()
        {
            var encodingResult = string.Join(";", FaceEncoding.Select(p => p.ToString()).ToArray());
            return new StringFaceDataBase { FaceEncodingString = encodingResult, Name = Name };            
        }
    }
    public class StringFaceDataBase
    {
        public string? Name { get; set; }
        [Key]
        public required string FaceEncodingString { get; set; }
        public RawFaceDataBase ToRawStringFaceDataBase()
        {
            var encodingResult = Array.ConvertAll(FaceEncodingString.Split(';'), double.Parse);
            return new RawFaceDataBase { FaceEncoding= encodingResult, Name = Name };
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