using System.Text.Json.Serialization;

namespace CheckInProject.Abstraction.Models
{
    public class FaceDataBase
    {
        public required string Name { get; set; }
        public required double[] FaceEncoding { get; set; }
    }
}