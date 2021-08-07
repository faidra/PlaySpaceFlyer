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

    readonly Dictionary<uint, Subject<Pose>> subjects = new Dictionary<uint, Subject<Pose>>();

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
            subject = new Subject<Pose>();
            subjects[deviceIndex] = subject;
        }
        return subject;
    }

    void Update()
    {
        var devicePoses = OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.GetDevicePoses();
        for (var i = 0; i < devicePoses.length; ++i)
        {
            var pose = devicePoses.devicePoses[i];
            if (!subjects.TryGetValue(pose.openVRID, out var subject)) continue;
            subject.OnNext(GetPose(pose));
        }
    }

    Pose GetPose(OpenVRSpaceCalibrator.OpenVRSpaceCalibrator.DevicePoses.DevicePose devicePose)
    {
        // OpenVR側に方法あるだろうけどとりあえず……
        Vector3 ConvertVector(double[] source) => OpenVRExtensions.FromRHandPosition(source[0], source[1], source[2]);
        Quaternion ConvertRotation(HmdQuaternion_t source) => OpenVRExtensions.FromRHandRotation(source.x, source.y, source.z, source.w);


        var position = ConvertVector(devicePose.vecPosition);
        var rotation = ConvertRotation(devicePose.qRotation);
        var worldFromDriverTranslation = ConvertVector(devicePose.vecWorldFromDriverTranslation);
        var worldFromDriverRotation = ConvertRotation(devicePose.qWorldFromDriverRotation);
        var driverFromHeadTranslation = ConvertVector(devicePose.vecDriverFromHeadTranslation);
        var driverFromHeadRotation = ConvertRotation(devicePose.qDriverFromHeadRotation);

        var worldFromDriverMatrix = Matrix4x4.TRS(worldFromDriverTranslation, worldFromDriverRotation, Vector3.one);
        var driverFromHeadMatrix = Matrix4x4.TRS(driverFromHeadTranslation, driverFromHeadRotation, Vector3.one);
        var deviceMatrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        var pose = worldFromDriverMatrix * deviceMatrix * driverFromHeadMatrix;
        return new Pose(pose.GetPosition(), pose.GetRotation());
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
