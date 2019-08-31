using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Valve.VR;
using System.IO;
using Debug = UnityEngine.Debug;

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
        Execute($"deviceoffsets {openVRDeviceId} enable");
        Execute($"deviceoffsets {openVRDeviceId} set worldPosOffset {pos.x} {pos.y} {pos.z}");
    }

    void DisableDeviceOffsets(uint openVRDeviceId)
    {
        Execute($"deviceoffsets {openVRDeviceId} disable");
        Execute($"deviceoffsets {openVRDeviceId} set worldPosOffset 0 0 0");
    }

    void Execute(string arguments)
    {
        if (processStartInfo == null) processStartInfo = new ProcessStartInfo(Path.Combine(Directory.GetCurrentDirectory(), CommandlineToolPath)) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true };
        processStartInfo.Arguments = arguments;
        var process = Process.Start(processStartInfo);
        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        if (!string.IsNullOrEmpty(output)) Debug.LogError($"{arguments}: {output}");
    }
}
