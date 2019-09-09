using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Valve.VR;
using System.Linq;

public class InputEmulator : MonoBehaviour
{
    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; }

    Vector3 referenceBaseStationPosition;

    ProcessStartInfo processStartInfo;
    VRInputEmulator inputSimulator;

    bool[] isDeviceOffsetEnabled = new bool[OpenVR.k_unMaxTrackedDeviceCount];

    void Start()
    {
        inputSimulator = new VRInputEmulator();
    }

    public Vector3 GetRealPosition(Vector3 virtualRawPosition)
    {
        return Quaternion.Inverse(CurrentRotation) * (virtualRawPosition - CurrentOffset - referenceBaseStationPosition) + referenceBaseStationPosition;
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
        OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated);
        TrackedDevicePose_t pose = default, gamePose = default;
        OpenVR.Compositor.GetLastPoseForTrackedDeviceIndex(deviceId, ref pose, ref gamePose);
        referenceBaseStationPosition = SteamVR_Utils.GetPosition(pose.mDeviceToAbsoluteTracking);
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
        EnforceDeviceOffsetEnabled(openVRDeviceId);
        inputSimulator.SetWorldFromDriverTranslationOffset(openVRDeviceId, pos, true);
    }

    void SetDeviceWorldRotOffset(uint openVRDeviceId, Quaternion rot)
    {
        EnforceDeviceOffsetEnabled(openVRDeviceId);
        inputSimulator.SetWorldFromDriverRotationOffset(openVRDeviceId, rot, true);
    }

    void EnforceDeviceOffsetEnabled(uint openVRDeviceId)
    {
        if (!isDeviceOffsetEnabled[openVRDeviceId])
        {
            inputSimulator.EnableDeviceOffsets(openVRDeviceId, true, true);
            isDeviceOffsetEnabled[openVRDeviceId] = true;
        }
    }

    void DisableDeviceOffsets(uint openVRDeviceId)
    {
        inputSimulator.EnableDeviceOffsets(openVRDeviceId, false, true);
        inputSimulator.SetWorldFromDriverTranslationOffset(openVRDeviceId, Vector3.zero, true);
        inputSimulator.SetWorldFromDriverRotationOffset(openVRDeviceId, Quaternion.identity, true);
        isDeviceOffsetEnabled[openVRDeviceId] = false;
    }
}
