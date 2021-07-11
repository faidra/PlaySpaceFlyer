using System.Runtime.InteropServices;

namespace OpenVRSpaceCalibrator
{
    public static class OpenVRSpaceCalibrator
    {
        const string dllName = "OpenVR-SpaceCalibratorClientDLL";

        [DllImport(dllName)]
        public static extern void Connect();

        [DllImport(dllName)]
        public static extern void ResetAndDisableDeviceTransform(uint deviceId);

        [DllImport(dllName)]
        public static extern void SetDeviceTransform(uint deviceId, double x, double y, double z, double qx, double qy, double qz, double qw);
    }
}
