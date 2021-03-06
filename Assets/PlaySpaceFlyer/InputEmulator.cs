﻿using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

public class InputEmulator : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; }

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
        if (pos == CurrentOffset && rot == CurrentRotation) return;
        foreach (var id in GetAllOpenVRDeviceIds()) SetDeviceTransform(id, pos, rot);
        CurrentOffset = pos;
        CurrentRotation = rot;
    }

    public void DisableAllDeviceTransform()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceTransform(id);
        CurrentOffset = Vector3.zero;
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
        var rpos = ToRHand(pos);
        var rrot = ToRHand(rot);
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

    static (double x, double y, double z) ToRHand(Vector3 vec)
    {
        return (vec.x, vec.y, -vec.z);
    }

    static (double x, double y, double z, double w) ToRHand(Quaternion rot)
    {
        return (-rot.x, -rot.y, rot.z, rot.w);
    }
}
