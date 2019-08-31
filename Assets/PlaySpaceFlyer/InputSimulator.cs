using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Valve.VR;

public class InputSimulator : MonoBehaviour
{
    [SerializeField]
    string CommandlineToolPath;

    ProcessStartInfo processStartInfo;

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
        DeviceOffsets(openVRDeviceId.ToString(), "enable");
        DeviceOffsets(openVRDeviceId.ToString(), "set", "worldPosOffset", pos.x.ToString(), pos.y.ToString(), pos.z.ToString());
    }

    void DisableDeviceOffsets(uint openVRDeviceId)
    {
        DeviceOffsets(openVRDeviceId.ToString(), "disable");
        DeviceOffsets(openVRDeviceId.ToString(), "set", "worldPosOffset", "0", "0", "0");
    }

    void DeviceOffsets(params string[] arguments)
    {
        ClientCommandline.deciveOffsets(arguments);
    }
}
