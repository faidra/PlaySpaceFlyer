﻿using UnityEngine;
using Valve.VR;
using System.Runtime.InteropServices;

public class HMD : MonoBehaviour
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    void Update()
    {
        var openvr = OpenVR.System;
        var pose = default(TrackedDevicePose_t);
        var state = default(VRControllerState_t);
        openvr.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, OpenVR.k_unTrackedDeviceIndex_Hmd, ref state, (uint) Marshal.SizeOf<VRControllerState_t>(), ref pose);

        Position = pose.mDeviceToAbsoluteTracking.GetPosition();
        Rotation = pose.mDeviceToAbsoluteTracking.GetRotation();
    }
}
