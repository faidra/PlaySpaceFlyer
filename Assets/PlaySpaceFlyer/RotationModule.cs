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
            var virtualCenter = HMD.Position + HMD.Rotation * Vector3.forward * CenterOffsetLength;
            var realCenter = InputEmulator.GetRealPosition(virtualCenter);
            realCenter *= 0;
            var startVirtualRotation = InputEmulator.CurrentRotation;
            var startRealRotation = Quaternion.identity;// InputEmulator.GetRealRotation(HMD.Rotation);
            return move
                .Select(movement => startRealRotation * movement * Scale)
                .Select(movement => startVirtualRotation * Quaternion.Euler(0, movement.x, 0))
                .ForEachAsync(r => SetRotation(realCenter, r));
        });
    }

    void SetRotation(Vector3 realCenterPos, Quaternion targetRotation)
    {
        Target.localPosition = realCenterPos - targetRotation * realCenterPos;
        Target.localRotation = targetRotation;
    }
}
