using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using Valve.VR;

public class RotationModule : MonoBehaviour
{
    [SerializeField] Toggle calibrationToggle;

    [SerializeField]
    Transform Target;
    [SerializeField]
    DoubleDrag2 source;
    [SerializeField]
    float BaseHeight;
    [SerializeField]
    float CenterOffsetMax;
    [SerializeField]
    float Scale;
    [SerializeField]
    ResetEvent ResetEvent;
    [SerializeField]
    Toggle useRotate;

    [SerializeField] PoseReceiver poseReceiver;


    void Start()
    {
        source.MovesAsObservable()
            .Where(_ => !calibrationToggle.isOn)
            .Where(_ => useRotate.isOn)
            .SelectMany(move => MoveToRotateAsObservable(move))
            .Subscribe()
            .AddTo(this);

        ResetEvent.OnResetAsObservable()
            .Where(_ => !calibrationToggle.isOn)
            .Subscribe(_ =>
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }).AddTo(this);
    }

    IObservable<Unit> MoveToRotateAsObservable(IObservable<Vector3> move)
    {
        return poseReceiver.OnPoseUpdatedAsObservable(SteamVR_Input_Sources.Head).FirstOrDefault()
            .ContinueWith(p =>
            {
                var length = Mathf.Min(BaseHeight / Mathf.Abs(Mathf.Tan(p.rotation.eulerAngles.x * Mathf.Deg2Rad)), CenterOffsetMax);
                var center = p.position + Quaternion.Euler(0, p.rotation.eulerAngles.y, 0) * Vector3.forward * length;

                var startLocalPosition = Target.localPosition;
                var startVirtualRotation = Target.localRotation;
                return Observable.Scan(move, startVirtualRotation,
                        (a, b) => a * Quaternion.Euler(0f, b.y * Scale * Time.deltaTime, 0f))
                    .ForEachAsync(r => SetRotation(startLocalPosition, startVirtualRotation, center, r));
            });
    }

    void SetRotation(Vector3 startLocalPosition, Quaternion startVirtualRotation, Vector3 realCenterPos, Quaternion targetRotation)
    {
        Target.localPosition = startLocalPosition + startVirtualRotation * realCenterPos - targetRotation * realCenterPos;
        Target.localRotation = targetRotation;
    }
}
