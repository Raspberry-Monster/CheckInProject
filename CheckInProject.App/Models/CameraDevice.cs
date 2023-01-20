using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInProject.App.Models
{
    public class CameraDevice
    {
        public required int OpenCvId { get; set; }
        public required string CameraName { get; set; }
        public required string DeviceId { get; set; }
    }
}
