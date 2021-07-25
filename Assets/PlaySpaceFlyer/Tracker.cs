using UniRx;
using UnityEngine;
using Valve.VR;

public sealed class Tracker : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources inputSource;
    [SerializeField] SteamVR_Action_Pose pose;
    [SerializeField] PoseReceiver poseReceiver;
    [SerializeField] float smoothTime;
    
    public bool IsActive { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; } = Quaternion.identity;
    
    public SteamVR_Input_Sources InputSources => SteamVR_Input_Sources.Head;

    Vector3 targetPosition;
    Vector3 vel;

    void Start()
    {
        poseReceiver.OnPoseUpdatedAsObservable(inputSource)
            .Subscribe(p =>
            {
                targetPosition = p.position;
                Rotation = p.rotation;
            }).AddTo(this);

        PoseVisualizer.Create(this, () => new PoseVisualizer.Param(IsActive, Position, Rotation, new Vector3(0.3f, 0.3f, 0.3f)));
    }

    void Update()
    {
        IsActive = pose.GetPoseIsValid(inputSource);
        Position = Vector3.SmoothDamp(Position, targetPosition, ref vel, smoothTime);
    }
}
