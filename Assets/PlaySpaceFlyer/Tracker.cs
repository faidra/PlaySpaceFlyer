using UnityEngine;
using Valve.VR;

public class Tracker : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources inputSource;
    [SerializeField] SteamVR_Action_Pose pose;
    
    public bool IsActive { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    void Update()
    {
        IsActive = pose.GetPoseIsValid(inputSource);
        Position = pose.GetLocalPosition(inputSource);
        Rotation = pose.GetLocalRotation(inputSource);
    }
}
