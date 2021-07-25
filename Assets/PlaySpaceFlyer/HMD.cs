using UnityEngine;
using Valve.VR;
using System.Runtime.InteropServices;
using UniRx;

public class HMD : MonoBehaviour
{
    [SerializeField] PoseReceiver poseReceiver;

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; } = Quaternion.identity;

    public SteamVR_Input_Sources InputSources => SteamVR_Input_Sources.Head;

    void Start()
    {
        poseReceiver.OnPoseUpdatedAsObservable(OpenVR.k_unTrackedDeviceIndex_Hmd)
            .Subscribe(p =>
            {
                Position = p.position;
                Rotation = p.rotation;
            }).AddTo(this);

        PoseVisualizer.Create(this, () => new PoseVisualizer.Param(true, Position, Rotation, new Vector3(0.3f, 0.3f, 0.3f)));
    }
}
