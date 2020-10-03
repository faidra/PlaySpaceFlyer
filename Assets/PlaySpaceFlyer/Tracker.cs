using UnityEngine;
using Valve.VR;
using System.Runtime.InteropServices;
using UniRx;

public class Tracker : MonoBehaviour
{
    public bool IsActive { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    [SerializeField] TrackerSelector trackerSelector;
    uint id;

    void Start()
    {
        trackerSelector.SelectedTrackingReferenceIdAsObservable()
            .Subscribe(id => this.id = id).AddTo(this);
    }

    void Update()
    {
        if (id == 0)
        {
            IsActive = false;
            return;
        }

        var openvr = OpenVR.System;
        var pose = default(TrackedDevicePose_t);
        var state = default(VRControllerState_t);
        var ok = openvr.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, id, ref state, (uint) Marshal.SizeOf<VRControllerState_t>(), ref pose);
        if (ok)
        {
            var transform = new SteamVR_Utils.RigidTransform(pose.mDeviceToAbsoluteTracking);
            Position = transform.pos;
            Rotation = transform.rot;
            IsActive = true;
        }
        else
        {
            IsActive = false;
        }
    }
}
