using UnityEngine;
using UniRx;
using System;

public class RotationModule : MonoBehaviour
{
    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform Target;
    [SerializeField]
    DoubleDrag Left;
    [SerializeField]
    HMD HMD;
    [SerializeField]
    float CenterOffsetLength;
    [SerializeField]
    float Scale;
    [SerializeField]
    ResetEvent ResetEvent;

    void Start()
    {
        Left.MovesAsObservable()
            .SelectMany(move => MoveToRotateAsObservable(move))
            .Subscribe()
            .AddTo(this);

        ResetEvent.OnResetAsObservable().Subscribe(_ =>
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }).AddTo(this);
    }

    IObservable<Unit> MoveToRotateAsObservable(IObservable<Vector3> move)
    {
        return Observable.Defer(() =>
        {
            var offset = Target.localPosition;
            var virtualCenter = HMD.Position + HMD.Rotation * Vector3.forward * CenterOffsetLength;
            var realCenter = InputEmulator.GetRealPosition(virtualCenter);
            var startVirtualRotation = InputEmulator.CurrentRotation;
            var startRealRotation = InputEmulator.GetRealRotation(HMD.Rotation);
            return Observable.Scan(move, (a, b) => a + b * Time.deltaTime)
                .Select(movement => Quaternion.Inverse(startRealRotation) * movement * Scale)
                .Select(movement => startVirtualRotation * Quaternion.Euler(0, movement.x, 0))
                .ForEachAsync(r => SetRotation(offset, startVirtualRotation, realCenter, r));
        });
    }

    void SetRotation(Vector3 offset, Quaternion startVirtualRotation, Vector3 realCenterPos, Quaternion targetRotation)
    {
        Target.localPosition = offset + startVirtualRotation * realCenterPos - targetRotation * realCenterPos;
        Target.localRotation = targetRotation;
    }
}
