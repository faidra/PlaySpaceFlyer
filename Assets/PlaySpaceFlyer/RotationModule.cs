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
    float BaseHeight;
    [SerializeField]
    float CenterOffsetMax;
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
            var length = Mathf.Min(BaseHeight / Mathf.Abs(Mathf.Tan(HMD.Rotation.eulerAngles.x* Mathf.Deg2Rad)), CenterOffsetMax);
            var virtualCenter = HMD.Position + Quaternion.Euler(0, HMD.Rotation.eulerAngles.y, 0) * Vector3.forward * length;
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
        var offsetedCenter = realCenterPos - InputEmulator.ReferenceBaseStationPosition;
        Target.localPosition = offset + startVirtualRotation * offsetedCenter - targetRotation * offsetedCenter;
        Target.localRotation = targetRotation;
    }
}
