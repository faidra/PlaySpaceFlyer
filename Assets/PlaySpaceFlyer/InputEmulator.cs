using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

public class InputEmulator : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; } = Quaternion.identity;

    readonly Dictionary<uint, (Vector3 pos, Quaternion rot)> lastUpdated = new Dictionary<uint, (Vector3 pos, Quaternion rot)>();

    void Start()
    {
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.Connect();
    }

    public Vector3 GetRealPosition(Vector3 virtualRawPosition)
    {
        return Quaternion.Inverse(CurrentRotation) * (virtualRawPosition - CurrentOffset);
    }

    public Quaternion GetRealRotation(Quaternion virtualRawRotation)
    {
        return Quaternion.Inverse(CurrentRotation) * virtualRawRotation;
    }

    public void SetAllDeviceTransform(Vector3 pos, Quaternion rot)
    {
        foreach (var id in GetAllOpenVRDeviceIds())
        {
            if (lastUpdated.TryGetValue(id, out var last) && last.pos == pos && last.rot == rot) continue;
            SetDeviceTransform(id, pos, rot);
            lastUpdated[id] = (pos, rot);
        }
        CurrentOffset = pos;
        CurrentRotation = rot;
    }

    public void DisableAllDeviceTransform()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceTransform(id);
        CurrentOffset = Vector3.zero;
        CurrentRotation = Quaternion.identity;
    }

    IEnumerable<uint> GetAllOpenVRDeviceIds()
    {
        for (var i = 0u; i < SteamVR.connected.Length; ++i)
        {
            if (SteamVR.connected[i]) yield return i;
        }
    }

    void SetDeviceTransform(uint openVRDeviceId, Vector3 pos, Quaternion rot)
    {
        var rpos = pos.ToRHand();
        var rrot = rot.ToRHand();
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.SetDeviceTransform(openVRDeviceId, rpos.x, rpos.y, rpos.z, rrot.x, rrot.y, rrot.z, rrot.w);
    }

    void DisableDeviceTransform(uint openVRDeviceId)
    {
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.ResetAndDisableDeviceTransform(openVRDeviceId);
    }

    void OnDestroy()
    {
        if (OpenVR.System == null) return;
        DisableAllDeviceTransform();
    }
}
