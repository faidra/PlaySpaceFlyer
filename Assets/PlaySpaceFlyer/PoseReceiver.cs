using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UniRx;
using UniRx.Triggers;

public sealed class PoseReceiver : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Pose dummyPose;

    public struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;

        public Pose(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    UdpReceiver udpReceiver;

    readonly Dictionary<uint, Subject<string[]>> subjects = new Dictionary<uint, Subject<string[]>>();

    public IObservable<Pose> OnPoseUpdatedAsObservable(SteamVR_Input_Sources inputSources)
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

    public IObservable<Pose> OnPoseUpdatedAsObservable(uint deviceIndex)
    {
        if (!subjects.TryGetValue(deviceIndex, out var subject))
        {
            subject = new Subject<string[]>();
            subjects[deviceIndex] = subject;
        }
        return subject.ThrottleFrame(0).Select(GetPose);
    }

    void Start()
    {
        udpReceiver = new UdpReceiver("127.0.0.1", 12345).AddTo(this);
        udpReceiver.OnReceivedAsObservable().Subscribe(Handle).AddTo(this);
    }

    void Handle(string str)
    {
        if (!TryGetId(str, out var id, out var splits)) return;
        if (!subjects.TryGetValue(id, out var subject)) return;
        subject.OnNext(splits);
    }

    Pose GetPose(string[] splits)
    {
        var position = OpenVRExtensions.FromRHandPosition(double.Parse(splits[1]), double.Parse(splits[2]), double.Parse(splits[3]));
        var rotation = OpenVRExtensions.FromRHandRotation(double.Parse(splits[4]), double.Parse(splits[5]), double.Parse(splits[6]), double.Parse(splits[7]));
        var worldFromDriverTranslation = OpenVRExtensions.FromRHandPosition(double.Parse(splits[8]), double.Parse(splits[9]), double.Parse(splits[10]));
        var worldFromDriverRotation = OpenVRExtensions.FromRHandRotation(double.Parse(splits[11]), double.Parse(splits[12]), double.Parse(splits[13]), double.Parse(splits[14]));
        var driverFromHeadTranslation = OpenVRExtensions.FromRHandPosition(double.Parse(splits[15]), double.Parse(splits[16]), double.Parse(splits[17]));
        var driverFromHeadRotation = OpenVRExtensions.FromRHandRotation(double.Parse(splits[18]), double.Parse(splits[19]), double.Parse(splits[20]), double.Parse(splits[21]));

        var worldFromDriverMatrix = Matrix4x4.TRS(worldFromDriverTranslation, worldFromDriverRotation, Vector3.one);
        var driverFromHeadMatrix = Matrix4x4.TRS(driverFromHeadTranslation, driverFromHeadRotation, Vector3.one);
        var devicePose = Matrix4x4.TRS(position, rotation, Vector3.one);
        var pose = worldFromDriverMatrix * devicePose * driverFromHeadMatrix;
        return new Pose(pose.GetPosition(), pose.GetRotation());
    }

    bool TryGetId(string str, out uint id, out string[] splits)
    {
        splits = str.Split(',');
        if (splits.Length <= 21)
        {
            id = default;
            return false;
        }
        return uint.TryParse(splits[0], out id);
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
