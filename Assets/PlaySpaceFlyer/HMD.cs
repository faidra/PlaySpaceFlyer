using UnityEngine;
using Valve.VR;
using UniRx;

public sealed class HMD : MonoBehaviour
{
    [SerializeField] PoseReceiver poseReceiver;
    [SerializeField] float smoothTime;

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; } = Quaternion.identity;

    public SteamVR_Input_Sources InputSources => SteamVR_Input_Sources.Head;

    Vector3 targetPosition;
    Vector3 vel;

    void Start()
    {
        poseReceiver.OnPoseUpdatedAsObservable(OpenVR.k_unTrackedDeviceIndex_Hmd)
            .Subscribe(p =>
            {
                targetPosition = p.position;
                Rotation = p.rotation;
            }).AddTo(this);

        PoseVisualizer.Create(this, () => new PoseVisualizer.Param(true, Position, Rotation, new Vector3(0.3f, 0.3f, 0.3f)));
    }

    void Update()
    {
        Position = Vector3.SmoothDamp(Position, targetPosition, ref vel, smoothTime);
    }
}
