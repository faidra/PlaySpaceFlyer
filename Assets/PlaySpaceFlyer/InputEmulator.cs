using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Valve.VR;

public class InputEmulator : MonoBehaviour
{
    [SerializeField] LighthouseOffset lighthouseOffset;

    public Vector3 CurrentOffset { get; private set; }
    public Quaternion CurrentRotation { get; private set; } = Quaternion.identity;
    public float CurrentScale { get; private set; } = 1f;

    readonly Dictionary<uint, (Vector3 pos, Quaternion rot)> lastUpdated = new Dictionary<uint, (Vector3 pos, Quaternion rot)>();

    void Start()
    {
        var er = new StringBuilder(256);
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.Connect(er);
        Debug.LogError(er.ToString());
    }

    static bool IsLightHouseDevice(uint id)
    {
        return GetTrackingSystemName(id).Equals("lighthouse") && id != OpenVR.k_unTrackedDeviceIndex_Hmd; // HMDを除くのは検証を楽にするため
    }

    static string GetTrackingSystemName(uint id)
    {
        var error = ETrackedPropertyError.TrackedProp_Success;
        var capacity = OpenVR.System.GetStringTrackedDeviceProperty(id, ETrackedDeviceProperty.Prop_TrackingSystemName_String, null, 0, ref error);
        if (capacity <= 1)
        {
            return "";
        }

        var buffer = new StringBuilder((int) capacity);
        OpenVR.System.GetStringTrackedDeviceProperty(id, ETrackedDeviceProperty.Prop_TrackingSystemName_String, buffer, capacity, ref error);
        return buffer.ToString();
    }

    public void SetAllDeviceTransform(Vector3 pos, Quaternion rot, float scale)
    {
        foreach (var id in GetAllOpenVRDeviceIds())
        {
            if (lastUpdated.TryGetValue(id, out var last) && last.pos == pos && last.rot == rot) continue;
            SetDeviceTransform(id, pos, rot, scale);
            lastUpdated[id] = (pos, rot);
        }
        CurrentOffset = pos;
        CurrentRotation = rot;
        CurrentScale = scale;
    }

    public void DisableAllDeviceTransform()
    {
        foreach (var id in GetAllOpenVRDeviceIds()) DisableDeviceTransform(id);
        CurrentOffset = Vector3.zero;
        CurrentRotation = Quaternion.identity;
        CurrentScale = 1f;
    }

    IEnumerable<uint> GetAllOpenVRDeviceIds()
    {
        for (var i = 0u; i < SteamVR.connected.Length; ++i)
        {
            if (SteamVR.connected[i]) yield return i;
        }
    }

    void SetDeviceTransform(uint openVRDeviceId, Vector3 pos, Quaternion rot, float scale)
    {
        if (IsLightHouseDevice(openVRDeviceId))
        {
            rot = rot * lighthouseOffset.CurrentRotation;
            pos = pos + lighthouseOffset.CurrentRotation * lighthouseOffset.CurrentOffset;
            // todo scale
        }

        var rpos = pos.ToRHand();
        var rrot = rot.ToRHand();
        OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.SetDeviceTransform(
            openVRDeviceId,
            rpos.x, rpos.y, rpos.z,
            rrot.x, rrot.y, rrot.z, rrot.w,
            scale);
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

    public void SetLighthouseOffset(Vector3 position, Quaternion rotation)
    {
        lighthouseOffset.SetValues(position, rotation);
        foreach (var id in GetAllOpenVRDeviceIds())
        {
            if (IsLightHouseDevice(id))
            {
                SetDeviceTransform(id, CurrentOffset, CurrentRotation, CurrentScale);
            }
        }
    }
}
