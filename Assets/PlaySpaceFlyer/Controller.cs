using System;
using UnityEngine;
using Valve.VR;
using UniRx;

public class Controller : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources inputSource;
    [SerializeField] SteamVR_Action_Vector2 stick;
    [SerializeField] SteamVR_Action_Pose pose;
    [SerializeField] SteamVR_Action_Boolean mainButton;
    [SerializeField] SteamVR_Action_Boolean modifier;
    [SerializeField] SteamVR_Action_Boolean canceller;

    public SteamVR_Input_Sources InputSources => inputSource;
    
    public Vector3 Position { get; private set; }
    readonly public ReactiveProperty<bool> MainButtonPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> ModifierPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> CancellerPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<Vector2> Stick = new ReactiveProperty<Vector2>();

    void Start()
    {
        var debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugCube.name = inputSource.ToString();
        debugCube.transform.localScale = Vector3.one * 0.2f;
        Disposable.Create(() => Destroy(debugCube)).AddTo(this);

        var debugCube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        debugCube2.name = inputSource.ToString();
        debugCube2.transform.localScale = Vector3.one * 0.2f;
        Disposable.Create(() => Destroy(debugCube2)).AddTo(this);

        FindObjectOfType<PoseReceiver>().OnPoseUpdatedAsObservable(inputSource)
            .Subscribe(p =>
            {
                Debug.LogError(p.pos.y - Position.y);
                debugCube.transform.position = p.pos;
                debugCube.transform.rotation = p.rot;

                debugCube2.transform.position = Position;
            }).AddTo(this);
    }
    
    void Update()
    {
        Position = pose.GetLocalPosition(inputSource);
        Stick.Value = stick.GetAxis(inputSource);
        MainButtonPressed.Value = mainButton.GetState(inputSource);
        ModifierPressed.Value = modifier.GetState(inputSource);
        CancellerPressed.Value = canceller.GetState(inputSource);
    }
}
