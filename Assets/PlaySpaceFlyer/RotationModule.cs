using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class RotationModule : MonoBehaviour
{
    [SerializeField]
    InputEmulator InputEmulator;
    [SerializeField]
    Transform Target;
    [SerializeField]
    DoubleDrag2 source;
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
    [SerializeField]
    Toggle useRotate;
    
    
    void Start()
    {
        source.MovesAsObservable()
            .Where(_ => useRotate.isOn)
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
            var startLocalPosition = Target.localPosition;
            var length = Mathf.Min(BaseHeight / Mathf.Abs(Mathf.Tan(HMD.Rotation.eulerAngles.x * Mathf.Deg2Rad)), CenterOffsetMax);
            var virtualCenter = HMD.Position + Quaternion.Euler(0, HMD.Rotation.eulerAngles.y, 0) * Vector3.forward * length;
            var realCenter = InputEmulator.GetRealPosition(virtualCenter);
            var startVirtualRotation = Target.localRotation;
            return Observable.Scan(move, startVirtualRotation,
                    (a, b) => a * Quaternion.Euler(0f, b.y * Scale * Time.deltaTime, 0f))
                .ForEachAsync(r => SetRotation(startLocalPosition, startVirtualRotation, realCenter, r));
        });
    }

    void SetRotation(Vector3 startLocalPosition, Quaternion startVirtualRotation, Vector3 realCenterPos, Quaternion targetRotation)
    {
        Target.localPosition = startLocalPosition + startVirtualRotation * realCenterPos - targetRotation * realCenterPos;
        Target.localRotation = targetRotation;
    }
}
