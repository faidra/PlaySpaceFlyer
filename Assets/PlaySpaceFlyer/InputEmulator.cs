using System;
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


    void DisableAllDeviceWorldPosOffset()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceOffsets(id);
        CurrentOffset = Vector3.zero;
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
        EnforceDeviceOffsetEnabled(openVRDeviceId);
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.SetDeviceOffset(openVRDeviceId, pos.x, pos.y, pos.z);
    }

    void SetDeviceWorldRotOffset(uint openVRDeviceId, Quaternion rot)
    {
        EnforceDeviceOffsetEnabled(openVRDeviceId);
        Debug.LogError("SetRot is not implemented");
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
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.ResetAndDisableOffsets(openVRDeviceId);
        isDeviceOffsetEnabled[openVRDeviceId] = false;
    }
}
