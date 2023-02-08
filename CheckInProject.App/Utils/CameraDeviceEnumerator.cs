using CheckInProject.App.Models;
using DirectShowLib;
using System.Collections.Generic;

namespace CheckInProject.App.Utils
{
    public static class CameraDeviceEnumerator
    {
        public static List<CameraDevice> EnumerateCameras()
        {
            var videoInputDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int openCvId = 0;
            var resultCameraDevices = new List<CameraDevice>();
            foreach (var device in videoInputDevices)
            {
                using (device)
                {
                    var cameraDevice = new CameraDevice()
                    {
                        CameraName = device.Name,
                        DeviceId = device.DevicePath,
                        OpenCvId = openCvId++
                    };
                    resultCameraDevices.Add(cameraDevice);
                }
            }
            return resultCameraDevices;
        }
    }
}
