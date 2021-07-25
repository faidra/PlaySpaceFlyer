using UnityEngine;
using Valve.VR;
using UniRx;

public sealed class Controller : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources inputSource;
    [SerializeField] SteamVR_Action_Vector2 stick;
    [SerializeField] SteamVR_Action_Pose pose;
    [SerializeField] SteamVR_Action_Boolean mainButton;
    [SerializeField] SteamVR_Action_Boolean modifier;
    [SerializeField] SteamVR_Action_Boolean canceller;
    [SerializeField] PoseReceiver poseReceiver;
    [SerializeField] float smoothTime;

    public SteamVR_Input_Sources InputSources => inputSource;
    
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    readonly public ReactiveProperty<bool> MainButtonPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> ModifierPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> CancellerPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<Vector2> Stick = new ReactiveProperty<Vector2>();

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
        
        PoseVisualizer.Create(this, () => new PoseVisualizer.Param(true, Position, Rotation, new Vector3(0.3f, 0.3f, 0.3f)));
    }

    void Update()
    {
        Stick.Value = stick.GetAxis(inputSource);
        MainButtonPressed.Value = mainButton.GetState(inputSource);
        ModifierPressed.Value = modifier.GetState(inputSource);
        CancellerPressed.Value = canceller.GetState(inputSource);
        Position = Vector3.SmoothDamp(Position, targetPosition, ref vel, smoothTime);
    }
}
