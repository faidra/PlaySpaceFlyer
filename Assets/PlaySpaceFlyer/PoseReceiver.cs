using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UniRx;
using UniRx.Triggers;

public sealed class PoseReceiver : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Pose dummyPose;
    
    UdpReceiver udpReceiver;

    readonly Dictionary<uint, Subject<(Vector3 pos, Quaternion rot)>> subjects = new Dictionary<uint, Subject<(Vector3 pos, Quaternion rot)>>();

    public IObservable<(Vector3 pos, Quaternion rot)> OnPoseUpdatedAsObservable(SteamVR_Input_Sources inputSources)
        => Observable.Defer(() =>
        {
            {
                if (TryGetDeviceIndex(inputSources, out var id)) return OnPoseUpdatedAsObservable(id);
            }
            return this.UpdateAsObservable()
                .FirstOrDefault(__ => TryGetDeviceIndex(inputSources, out _))
                .ContinueWith(_ =>
                {
                    TryGetDeviceIndex(inputSources, out var @id);
                    return OnPoseUpdatedAsObservable(@id);
                });
        });

    public IObservable<(Vector3 pos, Quaternion rot)> OnPoseUpdatedAsObservable(uint deviceIndex)
    {
        if (!subjects.TryGetValue(deviceIndex, out var subject))
        {
            subject = new Subject<(Vector3 pos, Quaternion rot)>();
            subjects[deviceIndex] = subject;
        }
        return subject.ThrottleFrame(0);
    }

    void Start()
    {
        udpReceiver = new UdpReceiver("127.0.0.1", 12345).AddTo(this);
        udpReceiver.OnReceivedAsObservable().Subscribe(Handle).AddTo(this);
    }

    void Handle(string str)
    {
        if (!TryParse(str, out var param)) return;
        if (!subjects.TryGetValue(param.deviceIndex, out var subject)) return;
        subject.OnNext((param.position, param.rotation));
    }

    readonly struct Parameter
    {
        public readonly uint deviceIndex;
        public readonly Vector3 position;
        public readonly Quaternion rotation;

        public Parameter(uint deviceIndex, Vector3 position, Quaternion rotation)
        {
            this.deviceIndex = deviceIndex;
            this.position = position;
            this.rotation = rotation;
        }
    }

    bool TryParse(string str, out Parameter param)
    {
        try
        {
            var prms = str.Split(',');
            if (prms.Length <= 7)
            {
                param = default;
                return false;
            }
            param = new Parameter(uint.Parse(prms[0]),
                OpenVRExtensions.FromRHandPosition(double.Parse(prms[1]), double.Parse(prms[2]), double.Parse(prms[3])),
                OpenVRExtensions.FromRHandRotation(double.Parse(prms[4]), double.Parse(prms[5]), double.Parse(prms[6]), double.Parse(prms[7])));
            return true;
        }
        catch
        {
            param = default;
            return false;
        }
    }

    bool TryGetDeviceIndex(SteamVR_Input_Sources inputSources, out uint id)
    {
        if (inputSources == SteamVR_Input_Sources.Head)
        {
            id = OpenVR.k_unTrackedDeviceIndex_Hmd;
            return true;
        }
        
        if (!dummyPose.GetActive(inputSources))
        {
            id = default;
            return false;
        }

        id = dummyPose.GetDeviceIndex(inputSources);
        return true;
    }
}
