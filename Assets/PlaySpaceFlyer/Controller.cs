﻿using UnityEngine;
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
    [SerializeField] PoseReceiver poseReceiver;

    public SteamVR_Input_Sources InputSources => inputSource;
    
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    readonly public ReactiveProperty<bool> MainButtonPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> ModifierPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<bool> CancellerPressed = new ReactiveProperty<bool>();
    readonly public ReactiveProperty<Vector2> Stick = new ReactiveProperty<Vector2>();

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
        Stick.Value = stick.GetAxis(inputSource);
        MainButtonPressed.Value = mainButton.GetState(inputSource);
        ModifierPressed.Value = modifier.GetState(inputSource);
        CancellerPressed.Value = canceller.GetState(inputSource);
    }
}
