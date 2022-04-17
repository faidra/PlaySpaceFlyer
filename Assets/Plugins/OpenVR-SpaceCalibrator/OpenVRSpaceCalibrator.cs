using System.Runtime.InteropServices;
using System.Text;
using Valve.VR;

namespace OpenVRSpaceCalibrator
{
    public static class OpenVRSpaceCalibrator
    {
        const string dllName = "OpenVR-SpaceCalibratorClientDLL";

        [DllImport(dllName)]
        public static extern void Connect(StringBuilder error);

        [DllImport(dllName)]
        public static extern void ResetAndDisableDeviceTransform(uint deviceId);

        [DllImport(dllName)]
        public static extern void SetDeviceTransform(
            uint deviceId,
            double px, double py, double pz,
            double qx, double qy, double qz, double qw,
            double scale);

        [DllImport(dllName)]
        public static extern DevicePoses GetDevicePoses();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DevicePoses
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public readonly struct DevicePose
            {
                public readonly uint openVRID;

                public readonly HmdQuaternion_t qWorldFromDriverRotation;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                public readonly double[] vecWorldFromDriverTranslation;

                public readonly HmdQuaternion_t qDriverFromHeadRotation;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                public readonly double[] vecDriverFromHeadTranslation;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                public readonly double[] vecPosition;
                public readonly HmdQuaternion_t qRotation;
            }

            public readonly uint length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public readonly DevicePose[] devicePoses;
        }
    }
}
