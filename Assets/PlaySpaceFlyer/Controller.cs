using UnityEngine;
using Valve.VR;
using System.Runtime.InteropServices;
using UniRx;

public class Controller : MonoBehaviour
{
    [SerializeField]
    ETrackedControllerRole ControllerRole;

    public Vector3 Position { get; private set; }
    readonly public ReactiveProperty<bool> MenuPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> PadPressed=new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> GripPressed=new ReactiveProperty<bool>();

    void Update()
    {
        var openvr = OpenVR.System;
        var deviceIndex = (openvr.GetTrackedDeviceIndexForControllerRole(ControllerRole));
        if (deviceIndex < 0 || OpenVR.k_unMaxTrackedDeviceCount <= deviceIndex)
        {
            Position = Vector3.zero;
            MenuPressed.Value = false;
            PadPressed.Value = false;
            GripPressed.Value = false;
            return;
        }
        var pose = default(TrackedDevicePose_t);
        var state = default(VRControllerState_t);
        openvr.GetControllerStateWithPose(ETrackingUniverseOrigin.TrackingUniverseStanding, deviceIndex, ref state, (uint)Marshal.SizeOf<VRControllerState_t>(), ref pose);

        var transform = new SteamVR_Utils.RigidTransform(pose.mDeviceToAbsoluteTracking);
        Position = transform.pos;
        MenuPressed.Value = Match(state.ulButtonPressed, EVRButtonId.k_EButton_ApplicationMenu);
        GripPressed.Value = Match(state.ulButtonPressed, EVRButtonId.k_EButton_Grip);
        PadPressed.Value = Match(state.ulButtonPressed, EVRButtonId.k_EButton_SteamVR_Touchpad);
    }

    bool Match(ulong ulButtonPressed, EVRButtonId buttonId)
    {
        return (ulButtonPressed & (1ul << (int)buttonId)) > 0;
    }
}
