using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Valve.VR;

public class InputEmulator : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }

    ProcessStartInfo processStartInfo;
    VRInputEmulator inputSimulator;

    bool[] isDeviceOffsetEnabled = new bool[OpenVR.k_unMaxTrackedDeviceCount];

    void Start()
    {
        inputSimulator = new VRInputEmulator();
    }

    public void SetAllDeviceWorldPosOffset(Vector3 pos)
    {
        if (pos == CurrentOffset) return;
        foreach (var id in GetAllOpenVRDeviceIds()) SetDeviceWorldPosOffset(id, pos);
        CurrentOffset = pos;
    }

    void DisableAllDeviceWorldPosOffset()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceOffsets(id);
        CurrentOffset = Vector3.zero;
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
        if (!isDeviceOffsetEnabled[openVRDeviceId])
        {
            inputSimulator.EnableDeviceOffsets(openVRDeviceId, true, true);
            isDeviceOffsetEnabled[openVRDeviceId] = true;
        }
        inputSimulator.SetWorldFromDriverTranslationOffset(openVRDeviceId, pos, true);
    }

    void DisableDeviceOffsets(uint openVRDeviceId)
    {
        inputSimulator.EnableDeviceOffsets(openVRDeviceId, false, true);
        inputSimulator.SetWorldFromDriverTranslationOffset(openVRDeviceId, Vector3.zero, true);
        isDeviceOffsetEnabled[openVRDeviceId] = false;
    }
}
