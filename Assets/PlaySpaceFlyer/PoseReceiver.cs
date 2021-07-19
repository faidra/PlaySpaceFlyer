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

    readonly Dictionary<uint, Subject<Parameter>> subjects = new Dictionary<uint, Subject<Parameter>>();

    public IObservable<Parameter> OnPoseUpdatedAsObservable(SteamVR_Input_Sources inputSources)
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

    public IObservable<Parameter> OnPoseUpdatedAsObservable(uint deviceIndex)
    {
        if (!subjects.TryGetValue(deviceIndex, out var subject))
        {
            subject = new Subject<Parameter>();
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
        subject.OnNext(param);
    }

    public readonly struct Parameter
    {
        public readonly uint deviceIndex;
        public readonly Vector3 position;
        public readonly Quaternion rotation;
        public readonly Vector3 worldFromDriverTranslation;
        public readonly Quaternion worldFromDriverRotation;
        public readonly Vector3 driverFromHeadTranslation;
        public readonly Quaternion driverFromHeadRotation;

        public Parameter(uint deviceIndex, Vector3 position, Quaternion rotation,
            Vector3 worldFromDriverTranslation, Quaternion worldFromDriverRotation,
            Vector3 driverFromHeadTranslation, Quaternion driverFromHeadRotation)
        {
            this.deviceIndex = deviceIndex;
            this.position = position;
            this.rotation = rotation;
            this.worldFromDriverTranslation = worldFromDriverTranslation;
            this.worldFromDriverRotation = worldFromDriverRotation;
            this.driverFromHeadTranslation = driverFromHeadTranslation;
            this.driverFromHeadRotation = driverFromHeadRotation;
        }
    }

    bool TryParse(string str, out Parameter param)
    {
        try
        {
            var prms = str.Split(',');
            if (prms.Length <= 21)
            {
                param = default;
                return false;
            }
            param = new Parameter(uint.Parse(prms[0]),
                OpenVRExtensions.FromRHandPosition(double.Parse(prms[1]), double.Parse(prms[2]), double.Parse(prms[3])),
                OpenVRExtensions.FromRHandRotation(double.Parse(prms[4]), double.Parse(prms[5]), double.Parse(prms[6]), double.Parse(prms[7])),
                OpenVRExtensions.FromRHandPosition(double.Parse(prms[8]), double.Parse(prms[9]), double.Parse(prms[10])),
                OpenVRExtensions.FromRHandRotation(double.Parse(prms[11]), double.Parse(prms[12]), double.Parse(prms[13]), double.Parse(prms[14])),
                OpenVRExtensions.FromRHandPosition(double.Parse(prms[15]), double.Parse(prms[16]), double.Parse(prms[17])),
                OpenVRExtensions.FromRHandRotation(double.Parse(prms[18]), double.Parse(prms[19]), double.Parse(prms[20]), double.Parse(prms[21])));
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
