using UniRx;
using UnityEngine;
using Valve.VR;

public class Tracker : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources inputSource;
    [SerializeField] SteamVR_Action_Pose pose;
    [SerializeField] PoseReceiver poseReceiver;
    
    public bool IsActive { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; } = Quaternion.identity;
    
    public SteamVR_Input_Sources InputSources => SteamVR_Input_Sources.Head;

    void Start()
    {
        poseReceiver.OnPoseUpdatedAsObservable(inputSource)
            .Subscribe(p =>
            {
                Position = p.position;
                Rotation = p.rotation;
            }).AddTo(this);
    }

    void Update()
    {
        IsActive = pose.GetPoseIsValid(inputSource);
    }
}
