﻿using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Valve.VR;

public class InputEmulator : MonoBehaviour
{
    ProcessStartInfo processStartInfo;
    VRInputEmulator inputSimulator;

    public Vector3 CurrentOffset { get; private set; }

    void Start()
    {
        inputSimulator = new VRInputEmulator();
    }

    public void SetAllDeviceWorldPosOffset(Vector3 pos)
    {
        foreach (var id in GetAllOpenVRDeviceIds()) SetDeviceWorldPosOffset(id, pos);
    }

    void DisableAllDeviceWorldPosOffset()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceOffsets(id);
    }

    void OnDestroy()
    {
        DisableAllDeviceWorldPosOffset();
        inputSimulator.Dispose();
    }

    IEnumerable<uint> GetAllOpenVRDeviceIds()
    {
        for (var i = 0u; i < OpenVR.k_unMaxTrackedDeviceCount; ++i)
        {
            if (OpenVR.System.IsTrackedDeviceConnected(i)) yield return i;
        }
    }

    void SetDeviceWorldPosOffset(uint openVRDeviceId, Vector3 pos)
    {
        inputSimulator.EnableDeviceOffsets(openVRDeviceId, true, true);
        inputSimulator.SetWorldFromDriverTranslationOffset(openVRDeviceId, pos, true);
        CurrentOffset = pos;
    }

    void DisableDeviceOffsets(uint openVRDeviceId)
    {
        inputSimulator.EnableDeviceOffsets(openVRDeviceId, false, true);
        inputSimulator.SetWorldFromDriverTranslationOffset(openVRDeviceId, Vector3.zero, true);
        CurrentOffset = Vector3.zero;
    }
}