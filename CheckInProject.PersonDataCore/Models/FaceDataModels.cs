using System.Drawing;

namespace CheckInProject.PersonDataCore.Models
{
    public class FaceRetangleModels
    {
        public required Bitmap RetangleImage { get; set; }
        public required IList<Bitmap> FaceImages { get; set; }
    }
    public class FaceCountModels
    {
        public required Bitmap RetangleImage { get; set; }
        public required int Count { get; set; }
    }
}
