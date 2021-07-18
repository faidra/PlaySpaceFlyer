using UnityEngine;
using Valve.VR;
using UniRx;

public static class OpenVRExtensions
{
    public static (double x, double y, double z) ToRHand(this Vector3 vec)
    {
        return (vec.x, vec.y, -vec.z);
    }

    public static (double x, double y, double z, double w) ToRHand(this Quaternion rot)
    {
        return (-rot.x, -rot.y, rot.z, rot.w);
    }

    public static Vector3 FromRHandPosition(double x, double y, double z)
    {
        return new Vector3((float) x, (float) y, -(float) z);
    }

    public static Quaternion FromRHandRotation(double x, double y, double z, double w)
    {
        return new Quaternion(-(float) x, -(float) y, (float) z, (float) w);
    }
}
