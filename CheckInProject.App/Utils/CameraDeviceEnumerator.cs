using CheckInProject.App.Models;
using DirectShowLib;
using System.Collections.Generic;
using System.Linq;

namespace CheckInProject.App.Utils
{
    public static class CameraDeviceEnumerator
    {
        public static List<CameraDevice> EnumerateCameras()
        {
            var cameras = new List<CameraDevice>();
            var videoInputDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int openCvId = 0;
            return videoInputDevices.Select(v => new CameraDevice()
            {
                DeviceId = v.DevicePath,
                CameraName = v.Name,
                OpenCvId = openCvId++
            }).ToList();
        }
    }
}
