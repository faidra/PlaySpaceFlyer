using System.Runtime.InteropServices;

namespace OpenVRSpaceCalibrator
{
    public static class OpenVRSpaceCalibrator
    {
        const string dllName = "OpenVR-SpaceCalibratorClientDLL";

        [DllImport(dllName)]
        public static extern void Connect();

        [DllImport(dllName)]
        public static extern void ResetAndDisableOffsets(uint deviceId);

        [DllImport(dllName)]
        public static extern void SetDeviceOffset(uint deviceId, double x, double y, double z);
    }
}
