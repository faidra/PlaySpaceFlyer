﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Valve.VR;
using System.Linq;
using Debug = UnityEngine.Debug;

public class InputEmulator : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; }

    public Vector3 ReferenceBaseStationPosition { get; private set; }

    ProcessStartInfo processStartInfo;

    bool[] isDeviceOffsetEnabled = new bool[OpenVR.k_unMaxTrackedDeviceCount];

    void Start()
    {
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.Connect();
    }

    public Vector3 GetRealPosition(Vector3 virtualRawPosition)
    {
        return Quaternion.Inverse(CurrentRotation) * (virtualRawPosition - CurrentOffset - ReferenceBaseStationPosition) + ReferenceBaseStationPosition;
    }

    public Quaternion GetRealRotation(Quaternion virtualRawRotation)
    {
        return Quaternion.Inverse(CurrentRotation) * virtualRawRotation;
    }

    public void SetAllDeviceWorldPosOffset(Vector3 pos)
    {
        if (pos == CurrentOffset) return;
        foreach (var id in GetAllOpenVRDeviceIds()) SetDeviceWorldPosOffset(id, pos);
        CurrentOffset = pos;
    }

    public void SetAllDeviceWorldRotOffset(Quaternion rot)
    {
        Debug.LogError("SetRot is not implemented");
        return;
        if (rot == CurrentRotation) return;
        foreach (var id in GetAllOpenVRDeviceIds()) SetDeviceWorldRotOffset(id, rot);
        CurrentRotation = rot;
    }

    public void SetReferenceBaseStation(uint deviceId)
    {
        TrackedDevicePose_t pose = default, gamePose = default;
        OpenVR.Compositor.GetLastPoseForTrackedDeviceIndex(deviceId, ref pose, ref gamePose);
        ReferenceBaseStationPosition = new SteamVR_Utils.RigidTransform(pose.mDeviceToAbsoluteTracking).pos;
    }

    public void DisableAllDeviceWorldPosOffset()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceOffsets(id);
        CurrentOffset = Vector3.zero;
    }

    void OnDestroy()
    {
        if (OpenVR.System == null) return;
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
        EnforceDeviceOffsetEnabled(openVRDeviceId);
        var rpos = ToRHand(pos);
        var rrot = ToRHand(Quaternion.identity);
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.SetDeviceTransform(openVRDeviceId, rpos.x, rpos.y, rpos.z, rrot.x, rrot.y, rrot.z, rrot.w);
    }

    void SetDeviceWorldRotOffset(uint openVRDeviceId, Quaternion rot)
    {
        throw new NotImplementedException();
        // EnforceDeviceOffsetEnabled(openVRDeviceId);
        // inputSimulator.SetWorldFromDriverRotationOffset(openVRDeviceId, rot, true);
    }

    void EnforceDeviceOffsetEnabled(uint openVRDeviceId)
    {
        if (!isDeviceOffsetEnabled[openVRDeviceId])
        {
            // inputSimulator.EnableDeviceOffsets(openVRDeviceId, true, true);
            isDeviceOffsetEnabled[openVRDeviceId] = true;
        }
    }

    void DisableDeviceOffsets(uint openVRDeviceId)
    {
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.ResetAndDisableDeviceTransform(openVRDeviceId);
        isDeviceOffsetEnabled[openVRDeviceId] = false;
    }

    static (double x, double y, double z) ToRHand(Vector3 vec)
    {
        return (vec.x, vec.y, -vec.z);
    }

    static (double w, double x, double y, double z) ToRHand(Quaternion rot)
    {
        return (rot.w, -rot.x, -rot.y, rot.z);
    }
}
