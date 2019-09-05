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

    void Start()
    {
        Left.MovesAsObservable()
            .SelectMany(move => MoveToRotateAsObservable(move))
            .Subscribe()
            .AddTo(this);
    }

    IObservable<Unit> MoveToRotateAsObservable(IObservable<Vector3> move)
    {
        return Observable.Defer(() =>
        {
            var virtualCenter = HMD.Position + HMD.Rotation * Vector3.forward * CenterOffsetLength;
            var realCenter = InputEmulator.GetRealPosition(virtualCenter);
            var startRotation = InputEmulator.GetRealRotation(HMD.Rotation);
            return move
                .Select(movement => HMD.Rotation * movement * Scale)
                .Select(movement => startRotation * Quaternion.Euler(movement.y, movement.x, 0))
                .ForEachAsync(r => SetRotation(realCenter, r));
        });
    }

    void SetRotation(Vector3 realCenterPos, Quaternion targetRotation)
    {
        Target.localPosition = realCenterPos - targetRotation * realCenterPos;
        Target.localRotation = targetRotation;
    }
}
